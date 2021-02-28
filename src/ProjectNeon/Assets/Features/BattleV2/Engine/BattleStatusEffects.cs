using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStatusEffects : OnMessage<StatusEffectResolved, PerformAction, CardResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delay = new FloatReference(0.5f);

    private readonly Queue<Member> _membersToProcess = new Queue<Member>();
    private Maybe<Member> _currentMember;
    private readonly Queue<IPayloadProvider> _currentMemberEffects = new Queue<IPayloadProvider>();
    private readonly Queue<ProposedReaction> _instantReactions = new Queue<ProposedReaction>();

    private bool _isProcessing;
    private bool _isProcessingStartOfTurn;

    public void ProcessStartOfTurnEffects()
    {
        _isProcessing = true;
        _isProcessingStartOfTurn = true;
        foreach (var member in state.Members.Values) 
            _membersToProcess.Enqueue(member);
        ResolveNext();
    }
    
    public void ProcessEndOfTurnEffects()
    {
        _isProcessing = true;
        _isProcessingStartOfTurn = false;
        foreach (var m in state.Members.Where(x => x.Value.IsConscious())) 
            _membersToProcess.Enqueue(m.Value);
        this.ExecuteAfterDelay(ResolveNext, delay);
    }

    private void ResolveNext()
    {
        if (_instantReactions.Any())
            ResolveNextInstantReaction();
        else if (_currentMemberEffects.Any())
            ResolveNextStatusEffect();
        else if (_membersToProcess.Any())
            ResolveNextMemberStatusEffects();
        else
        {
            _currentMember = Maybe<Member>.Missing();
            _isProcessing = false;
            if (_isProcessingStartOfTurn)
                Message.Publish(new StartOfTurnEffectsStatusResolved());
            else
                Message.Publish(new EndOfTurnStatusEffectsResolved());
        }
    }

    private void ResolveNextInstantReaction()
    { 
        var r = _instantReactions.Dequeue();
        r.ReactionSequence.Perform(r.Name, r.Source, r.Target, ResourceQuantity.None);
    }

    private void ResolveNextStatusEffect()
    {
        var e = _currentMemberEffects.Dequeue();
        DevLog.Write($"Effect is Finished {e.IsFinished()}");
        if (!e.IsFinished())
        {
            var member = _currentMember.Value;
            var battleSnapshotBefore = state.GetSnapshot();

            MessageGroup.Start(e, () => 
            {
                var battleSnapshotAfter = state.GetSnapshot();
                var effectResolved = new EffectResolved(EffectData.Nothing, 
                    member, // Replace with Status Originator
                    new Single(member), 
                    battleSnapshotBefore,
                    battleSnapshotAfter, 
                    false);
                Message.Publish(new StatusEffectResolved(member, effectResolved));
            });
        }
        else
        {
            this.ExecuteAfterDelay(() =>
            {
                _currentMember.Value.State.CleanExpiredStates();
                ResolveNext();
            }, delay);
        }
    }
    
    private void ResolveNextMemberStatusEffects()
    {
        var member = _membersToProcess.Dequeue();
        _currentMember = member;
        
        if (!member.State.HasAnyTemporalStates)
        {
            ResolveNext();
        }
        else
        {
            var effects = _isProcessingStartOfTurn
                ? member.State.GetTurnStartEffects()
                : member.State.GetTurnEndEffects();
            if (effects.Length > 0)
                DevLog.Write($"Resolving {effects.Length} Status Effects for {member.Name}");
            effects.Where(e => !e.IsFinished())
                .ForEach(e => _currentMemberEffects.Enqueue(e));
            ResolveNext();
        }
    }

    protected override void Execute(StatusEffectResolved msg)
    {
        Debug.Log("Status Effect Resolved");
        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(msg.EffectResolved)).ToList();
        
        var immediateReactions = reactions.Where(r => r.ReactionCard.IsMissing);
        immediateReactions.ForEach(r => _instantReactions.Enqueue(r));
        
        msg.Member.State.CleanExpiredStates();
        ResolveNext();
    }

    protected override void Execute(PerformAction msg)
    {
        msg.Action();
        Message.Publish(new Finished<PerformAction>());
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        DevLog.Write("Card Resolution Finished");
        if (_isProcessing)
            ResolveNext();
    }
}
