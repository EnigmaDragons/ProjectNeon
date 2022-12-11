using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private readonly TemporalStateTracker _tracker;
    private readonly Func<EffectResolved, Maybe<ProposedReaction>> _createMaybeEffect;
    private readonly bool _hasUnlimitedUsesPerTurn;
    private readonly int _maxUsesPerTurn;
    
    public ReactionTimingWindow Timing { get; }
    private int _usesThisTurn;
    
    public int OriginatorId => _tracker.Metadata.OriginatorId;
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;
    private bool HasUsesRemainingThisTurn => _hasUnlimitedUsesPerTurn || _usesThisTurn < _maxUsesPerTurn;

    public ReactiveEffectV2Base(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, ReactionTimingWindow timing, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(timing, new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), createMaybeEffect) {}
    public ReactiveEffectV2Base(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, ReactionTimingWindow timing, int maxUsesPerTurn, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(timing, new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), maxUsesPerTurn <= 0, maxUsesPerTurn, createMaybeEffect) {}
    public ReactiveEffectV2Base(ReactionTimingWindow timing, TemporalStateMetadata metadata, Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
        : this(timing, metadata, true, -1, createMaybeEffect) {}

    private ReactiveEffectV2Base(ReactionTimingWindow timing, TemporalStateMetadata metadata,
        bool hasUnlimitedUsesPerTurn, int maxUsesPerTurn,
        Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect)
    {
        Timing = timing;
        _tracker = new TemporalStateTracker(metadata);
        _hasUnlimitedUsesPerTurn = hasUnlimitedUsesPerTurn;
        _maxUsesPerTurn = maxUsesPerTurn;
        _createMaybeEffect = createMaybeEffect;
    }
    
    public IPayloadProvider OnTurnStart()
    {
        _usesThisTurn = 0;
        return new NoPayload();
    }

    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() =>
        new ClonedReactiveEffect(Timing, _tracker.Metadata, _createMaybeEffect);


    public Maybe<ProposedReaction> OnEffectResolved(EffectResolved e)
    {
        if (!_tracker.IsActive || !HasUsesRemainingThisTurn)
            return Maybe<ProposedReaction>.Missing();
        
        var maybeEffect = _createMaybeEffect(e);
        if (maybeEffect.IsPresent)
        {
            _tracker.RecordUse();
            _usesThisTurn++;
        }
        return maybeEffect;
    }
    
    protected static Func<EffectResolved, Maybe<ProposedReaction>> CreateMaybeEffect(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        ReactionCardType reaction, ReactionTimingWindow timing, Func<EffectResolved, bool> condition) => 
            effect =>
            {
                var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
                if (!ReactionIsApplicable(possessor, canReactToReactions, effect, condition))
                    return Maybe<ProposedReaction>.Missing();

                var action = reaction.ActionSequence;
                var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
                var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
                if (target.Members.Any())
                    return new ProposedReaction(reaction, reactor, target, timing);
                return Maybe<ProposedReaction>.Missing();
            };
    
    protected static Func<EffectResolved, Maybe<ProposedReaction>> CreateMaybeEffect(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        CardReactionSequence reaction, ReactionTimingWindow timing, Func<EffectResolved, bool> condition) => 
        effect =>
        {
            var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
            if (!ReactionIsApplicable(possessor, canReactToReactions, effect, condition))
                return Maybe<ProposedReaction>.Missing();

            var action = reaction;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
            var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
            if (target.Members.Any())
                return new ProposedReaction(reaction, reactor, target, timing);
            return Maybe<ProposedReaction>.Missing();
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
        if (action.Scope == ReactiveTargetScope.OneRandomAlly)
            target = new Multiple(members.Values.ToArray().GetConsciousAllies(reactor).Shuffled().Take(1));
        if (action.Scope == ReactiveTargetScope.HealthiestAllyExceptSelf)
            target = new Multiple(members.Values.ToArray().GetConsciousNonSelfAllies(reactor).Shuffled().OrderByDescending(x => x.CurrentHp()).Take(1));
        if (action.Scope == ReactiveTargetScope.UnhealthiestAllyExceptSelf)
            target = new Multiple(members.Values.ToArray().GetConsciousNonSelfAllies(reactor).Shuffled().OrderByDescending(x => x.MissingHp()).Take(1));
        if (action.Scope == ReactiveTargetScope.UnhealthiestByPercentageAllyExceptSelf)
            target = new Multiple(members.Values.ToArray().GetConsciousNonSelfAllies(reactor).Shuffled().OrderBy(x => (x.CurrentHp() + x.CurrentShield()) / (x.MaxHp() + x.State.StartingShield())).Take(1));
        if (action.Scope == ReactiveTargetScope.OneRandomEnemy)
            target = new Multiple(members.Values.ToArray().GetConsciousEnemies(reactor).Shuffled().Take(1));
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
    public ClonedReactiveEffect(ReactionTimingWindow timing, TemporalStateMetadata metadata, 
        Func<EffectResolved, Maybe<ProposedReaction>> createMaybeEffect) 
            : base(timing, metadata, createMaybeEffect) {}
}
