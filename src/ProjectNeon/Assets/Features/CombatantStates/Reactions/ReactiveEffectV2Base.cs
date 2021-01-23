using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private readonly int _maxDurationTurns;
    private readonly int _maxUses;

    private int _remainingDurationTurns;
    private int _remainingUses;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;
    private readonly Func<CardActionAvoided, Maybe<ProposedReaction>> _createMaybeAvoidedEffect;
    private bool HasMoreUses => _remainingUses != 0;
    private bool HasMoreTurns => _remainingDurationTurns != 0;

    public IStats Stats => new StatAddends();
    public abstract StatusDetail Status { get; }
    public bool IsDebuff { get; }
    public bool IsActive => HasMoreUses && HasMoreTurns;
    public Maybe<int> Amount => _remainingUses;
    public Maybe<int> RemainingTurns => _remainingDurationTurns;

    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(isDebuff, maxDurationTurns, maxUses, createMaybeEffect, e => Maybe<ProposedReaction>.Missing()) {}
    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, Func<CardActionAvoided, Maybe<ProposedReaction>> createMaybeAvoidedEffect)
        : this(isDebuff, maxDurationTurns, maxUses, e => Maybe<ProposedReaction>.Missing(), createMaybeAvoidedEffect) {}
    public ReactiveEffectV2Base(bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect, 
        Func<CardActionAvoided, Maybe<ProposedReaction>> createMaybeAvoidedEffect)
    {
        _maxDurationTurns = maxDurationTurns;
        _maxUses = maxUses;
        _remainingDurationTurns = maxDurationTurns;
        _remainingUses = maxUses;
        _createMaybeEffect = createMaybeEffect;
        _createMaybeAvoidedEffect = createMaybeAvoidedEffect;
        IsDebuff = isDebuff;
        if (!IsActive)
            Log.Error($"{GetType()} was created inactive with {maxUses} Uses and {maxDurationTurns} Turns");
    }
    
    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        if (_remainingDurationTurns > 0)
            _remainingDurationTurns--;
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() =>
        new ClonedReactiveEffect(Status, IsDebuff, _maxDurationTurns, _maxUses, _createMaybeEffect);
    
    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        if (!IsActive)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
            _remainingUses--;
        return maybeEffect;
    }
    
    public Maybe<ProposedReaction> OnCardActionAvoided(CardActionAvoided e)
    {
        if (!IsActive)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeAvoidedEffect(e);
        if (maybeEffect.IsPresent)
            _remainingUses--;
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
            var possessor = members[possessingMemberId];
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
    public ClonedReactiveEffect(StatusDetail status, bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect) 
        : base(isDebuff, maxDurationTurns, maxUses, createMaybeEffect)
    {
        Status = status;
    }

    public override StatusDetail Status { get; }
}
