using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private readonly TemporalStateTracker _tracker;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;

    public int OriginatorId => _tracker.Metadata.OriginatorId;
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;

    public ReactiveEffectV2Base(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), createMaybeEffect) {}
    public ReactiveEffectV2Base(TemporalStateMetadata metadata, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
    {
        _tracker = new TemporalStateTracker(metadata);
        _createMaybeEffect = createMaybeEffect;
    }
    
    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() =>
        new ClonedReactiveEffect(_tracker.Metadata, _createMaybeEffect);
    
    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        if (!_tracker.IsActive)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
            _tracker.RecordUse();
        return maybeEffect;
    }
    
    protected static Func<EffectResolved, Maybe<ProposedReaction>> CreateMaybeEffect(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        ReactionCardType reaction, Func<EffectResolved, bool> condition) => 
            effect =>
            {
                var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
                if (!ReactionIsApplicable(possessor, canReactToReactions, effect, condition))
                    return Maybe<ProposedReaction>.Missing();

                var action = reaction.ActionSequence;
                var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
                var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
                return new ProposedReaction(reaction, reactor, target);
            };
    
    protected static Func<EffectResolved, Maybe<ProposedReaction>> CreateMaybeEffect(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        CardReactionSequence reaction, Func<EffectResolved, bool> condition) => 
        effect =>
        {
            var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
            if (!ReactionIsApplicable(possessor, canReactToReactions, effect, condition))
                return Maybe<ProposedReaction>.Missing();

            var action = reaction;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
            var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
            return new ProposedReaction(reaction, reactor, target);
        };

    private static Target GetReactionTarget(Member possessor, Member reactor, IDictionary<int, Member> members, CardReactionSequence action, Member effectSource, Target effectTarget)
    {
        Target target = new Single(reactor);
        if (action.Scope == ReactiveTargetScope.Possessor)
            target = new Single(possessor);
        if (action.Scope == ReactiveTargetScope.Source)
            target = new Single(effectSource);
        if (action.Scope == ReactiveTargetScope.Target)
            target = effectTarget;
        if (action.Scope == ReactiveTargetScope.AllEnemies)
            target = new Multiple(members.Values.ToArray().GetConsciousEnemies(reactor));
        if (action.Scope == ReactiveTargetScope.AllAllies)
            target = new Multiple(members.Values.ToArray().GetConsciousAllies(reactor));
        if (action.Scope == ReactiveTargetScope.Everyone)
            target = new Multiple(members.Values.Where(x => x.IsConscious()).ToArray());
        return target;
    }

    private static bool ReactionIsApplicable(Member possessor, bool canReactToReactions, EffectResolved effect, Func<EffectResolved, bool> condition)
    {
        if (effect.IsReaction && !canReactToReactions)
            return false;
        if (!condition(effect))
            return false;
        return true;
    }
}

public class ClonedReactiveEffect : ReactiveEffectV2Base
{
    public ClonedReactiveEffect(TemporalStateMetadata metadata, 
        Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect) 
            : base(metadata, createMaybeEffect) {}
}
