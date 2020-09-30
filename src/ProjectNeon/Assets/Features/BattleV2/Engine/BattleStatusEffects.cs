using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStatusEffects : OnMessage<StatusEffectResolved>
{
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delay = new FloatReference(0.8f);

    private readonly Queue<Member> _membersToProcess = new Queue<Member>();
    private bool _isProcessingStartOfTurn;
    
    public void ProcessStartOfTurnEffects()
    {
        _isProcessingStartOfTurn = true;
        foreach (var member in state.Members.Values) 
            _membersToProcess.Enqueue(member);
        ResolveNext();
    }
    
    public void ProcessEndOfTurnEffects()
    {
        _isProcessingStartOfTurn = false;
        foreach (var member in state.Members.Values) 
            _membersToProcess.Enqueue(member);
        this.ExecuteAfterDelay(ResolveNext, delay);
    }

    private void ResolveNext()
    {
        if (_membersToProcess.Any())
        {
            BattleLog.Write("Requested Resolve Status Effect");   
            var member = _membersToProcess.Dequeue();
            var effectPayloadProvider = _isProcessingStartOfTurn
                ? member.State.GetTurnStartEffects()
                : member.State.GetTurnEndEffects();
            if (!effectPayloadProvider.IsFinished())
            {
                SequenceMessage.Queue(effectPayloadProvider);
                
                Message.Subscribe<SequenceFinished>(_ =>
                {
                    Message.Unsubscribe(this);
                    Message.Subscribe<StatusEffectResolved>(Execute, this);
                    Message.Publish(new StatusEffectResolved(member));;
                }, this);
            }
            else
            {
                this.ExecuteAfterDelay(() =>
                {
                    member.State.CleanExpiredStates();
                    ResolveNext(); 
                }, member.State.HasAnyTemporalStates ? delay : 0f);
            }
        }
        else
        {
            if (_isProcessingStartOfTurn)
                Message.Publish(new StartOfTurnEffectsStatusResolved());
            else
                Message.Publish(new EndOfTurnStatusEffectsResolved());
        }
    }

    protected override void Execute(StatusEffectResolved msg)
    {
        msg.Member.State.CleanExpiredStates();
        this.ExecuteAfterDelay(ResolveNext, delay);
    }
}
