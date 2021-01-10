using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStatusEffects : OnMessage<StatusEffectResolved>
{
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delay = new FloatReference(0.5f);

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
        foreach (var m in state.Members.Where(x => x.Value.IsConscious())) 
            _membersToProcess.Enqueue(m.Value);
        this.ExecuteAfterDelay(ResolveNext, delay);
    }

    private void ResolveNext()
    {
        if (_membersToProcess.Any())
            ResolveNextMemberStatusEffects();
        else
        {
            if (_isProcessingStartOfTurn)
                Message.Publish(new StartOfTurnEffectsStatusResolved());
            else
                Message.Publish(new EndOfTurnStatusEffectsResolved());
        }
    }

    private void ResolveNextMemberStatusEffects()
    {
        var member = _membersToProcess.Dequeue();
        if (!member.State.HasAnyTemporalStates)
        {
            ResolveNext();
        }
        else
        {
            var effectPayloadProvider = _isProcessingStartOfTurn
                ? member.State.GetTurnStartEffects()
                : member.State.GetTurnEndEffects();
            if (effectPayloadProvider.Count > 0)
                DevLog.Write($"Resolving {effectPayloadProvider.Count} Status Effects for {member.Name}");
            if (!effectPayloadProvider.IsFinished())
            {
                Message.Subscribe<SequenceFinished>(_ =>
                {
                    Message.Unsubscribe(this);
                    Message.Subscribe<StatusEffectResolved>(Execute, this);
                    Message.Publish(new StatusEffectResolved(member));
                }, this);
                
                SequenceMessage.Queue(effectPayloadProvider);
            }
            else
            {
                this.ExecuteAfterDelay(() =>
                {
                    member.State.CleanExpiredStates();
                    ResolveNext();
                }, delay);
            }
        }
    }

    protected override void Execute(StatusEffectResolved msg)
    {
        msg.Member.State.CleanExpiredStates();
        this.ExecuteAfterDelay(ResolveNext, delay);
    }
}
