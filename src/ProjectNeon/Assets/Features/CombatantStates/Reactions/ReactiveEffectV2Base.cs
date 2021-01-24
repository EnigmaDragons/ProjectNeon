using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private readonly TemporalStateTracker _tracker;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;
    private readonly Func<CardActionAvoided, Maybe<ProposedReaction>> _createMaybeAvoidedEffect;

    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;

    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(new TemporalStateMetadata(isDebuff, maxUses, maxDurationTurns, status), createMaybeEffect, e => Maybe<ProposedReaction>.Missing()) {}
    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, Func<CardActionAvoided, Maybe<ProposedReaction>> createMaybeAvoidedEffect)
        : this(new TemporalStateMetadata(isDebuff, maxUses, maxDurationTurns, status), e => Maybe<ProposedReaction>.Missing(), createMaybeAvoidedEffect) {}
    public ReactiveEffectV2Base(TemporalStateMetadata metadata,
        Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect, 
        Func<CardActionAvoided, Maybe<ProposedReaction>> createMaybeAvoidedEffect)
    {
        _tracker = new TemporalStateTracker(metadata);
        _createMaybeEffect = createMaybeEffect;
        _createMaybeAvoidedEffect = createMaybeAvoidedEffect;
    }
    
    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() =>
        new ClonedReactiveEffect(_tracker.Metadata, _createMaybeEffect, _createMaybeAvoidedEffect);
    
    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        if (!_tracker.IsActive)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
            _tracker.RecordUse();
        return maybeEffect;
    }
    
    public Maybe<ProposedReaction> OnCardActionAvoided(CardActionAvoided e)
    {
        if (!_tracker.IsActive)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeAvoidedEffect(e);
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
        Target target = new Single(possessor);
        if (action.Scope == ReactiveTargetScope.Possessor)
            target = target;
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
        // Noah's super hack for OnDeath changes values during the Condition Resolution above ^
        if (!possessor.IsConscious())
            return false;
        return true;
    }
    
    protected static Func<CardActionAvoided, Maybe<ProposedReaction>> CreateMaybeAvoidedEffect(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, 
        ReactionCardType reaction, Func<CardActionAvoided, bool> condition) => 
        effect =>
        {
            var avoidedMaybeMember = effect.AvoidingMembers.Where(m => m.Id == possessingMemberId);
            if (avoidedMaybeMember.None() || !condition(effect))
                return Maybe<ProposedReaction>.Missing();

            var possessor = avoidedMaybeMember.First();
            if (!possessor.IsConscious())
                return Maybe<ProposedReaction>.Missing();
                
            var action = reaction.ActionSequence;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
            var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
            return new ProposedReaction(reaction, reactor, target);
        };
}

public class ClonedReactiveEffect : ReactiveEffectV2Base
{
    public ClonedReactiveEffect(TemporalStateMetadata metadata, 
        Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect,
        Func<CardActionAvoided, Maybe<ProposedReaction>> createMaybeAvoidedEffect) 
        : base(metadata, createMaybeEffect, createMaybeAvoidedEffect) {}
}
