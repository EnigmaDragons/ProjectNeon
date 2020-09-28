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
            Message.Subscribe<SequenceFinished>(_ =>
            {
                Message.Unsubscribe(this);
                Message.Subscribe<StatusEffectResolved>(Execute, this);
                Message.Publish(new StatusEffectResolved());;
            }, this);
            
            var member = _membersToProcess.Dequeue().State;
            var effectPayloadProvider = _isProcessingStartOfTurn
                ? member.GetTurnStartEffects()
                : member.GetTurnEndEffects();
            if (!effectPayloadProvider.IsFinished())
                SequenceMessage.Queue(effectPayloadProvider);
            else
                ResolveNext();
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
        this.ExecuteAfterDelay(ResolveNext, delay);
    }
}
