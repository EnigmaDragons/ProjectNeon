using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ReactiveEffectV2Base : ReactiveStateV2
{
    private readonly TemporalStateTracker _tracker;
    private readonly Func<EffectResolved, ProposedReaction[]> _createMaybeEffects;
    private readonly bool _hasUnlimitedUsesPerTurn;
    private readonly int _maxUsesPerTurn;
    
    public ReactionTimingWindow Timing { get; }
    public bool OnlyReactDuringCardPhases { get; }
    private int _usesThisTurn;
    
    public int OriginatorId => _tracker.Metadata.OriginatorId;
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public bool IsDot => false;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;
    private bool HasUsesRemainingThisTurn => _hasUnlimitedUsesPerTurn || _usesThisTurn < _maxUsesPerTurn;

    public ReactiveEffectV2Base(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, Func<EffectResolved, ProposedReaction[]> createMaybeEffects)
        : this(timing, onlyReactDuringCardPhases, new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), createMaybeEffects) {}
    public ReactiveEffectV2Base(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, int maxUsesPerTurn, Func<EffectResolved, ProposedReaction[]> createMaybeEffects)
        : this(timing, onlyReactDuringCardPhases, new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), maxUsesPerTurn <= 0, maxUsesPerTurn, createMaybeEffects) {}
    public ReactiveEffectV2Base(ReactionTimingWindow timing, bool onlyReactDuringCardPhases, TemporalStateMetadata metadata, Func<EffectResolved, ProposedReaction[]> createMaybeEffects)
        : this(timing, onlyReactDuringCardPhases, metadata, true, -1, createMaybeEffects) {}

    private ReactiveEffectV2Base(ReactionTimingWindow timing, bool onlyReactDuringCardPhases, TemporalStateMetadata metadata,
        bool hasUnlimitedUsesPerTurn, int maxUsesPerTurn,
        Func<EffectResolved, ProposedReaction[]> createMaybeEffects)
    {
        Timing = timing;
        OnlyReactDuringCardPhases = onlyReactDuringCardPhases;
        _tracker = new TemporalStateTracker(metadata);
        _hasUnlimitedUsesPerTurn = hasUnlimitedUsesPerTurn;
        _maxUsesPerTurn = maxUsesPerTurn;
        _createMaybeEffects = createMaybeEffects;
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
        new ClonedReactiveEffect(Timing, OnlyReactDuringCardPhases, _tracker.Metadata, _createMaybeEffects);


    public ProposedReaction[] OnEffectResolved(EffectResolved e)
    {
        if (!_tracker.IsActive || !HasUsesRemainingThisTurn)
            return Array.Empty<ProposedReaction>();
        
        var maybeEffects = _createMaybeEffects(e);
        if (maybeEffects.Length > 0)
        {
            _tracker.RecordUse();
            _usesThisTurn++;
        }
        return maybeEffects;
    }
    
    protected static Func<EffectResolved, ProposedReaction[]> CreateMaybeEffects(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        ReactionCardType reaction, ReactionTimingWindow timing, Func<EffectResolved, int> condition) => 
            effect =>
            {
                var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
                if (!ReactionIsApplicable(possessor, canReactToReactions, effect, condition(effect)))
                    return Array.Empty<ProposedReaction>();

                var action = reaction.ActionSequence;
                var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
                var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
                if (target.Members.Any())
                    return new [] {new ProposedReaction(reaction, reactor, target, timing)};
                return Array.Empty<ProposedReaction>();
            };
    
    protected static Func<EffectResolved, ProposedReaction[]> CreateMaybeEffects(
        IDictionary<int, Member> members, int possessingMemberId, Member originator, bool canReactToReactions,
        CardReactionSequence reaction, ReactionTimingWindow timing, Func<EffectResolved, int> condition) => 
        effect =>
        {
            var possessor = members.VerboseGetValue(possessingMemberId, "Reaction Possessing Member");
            var triggerCount = condition(effect);
            if (!ReactionIsApplicable(possessor, canReactToReactions, effect, triggerCount))
                return Array.Empty<ProposedReaction>();

            var action = reaction;
            var reactor = action.Reactor == ReactiveMember.Originator ? originator : possessor;
            var reactions = new List<ProposedReaction>();
            for (var i = 0; i < (reaction.MultiTrigger ? triggerCount : 1); i++)
            {
                var target = GetReactionTarget(possessor, reactor, members, action, effect.Source, effect.Target);
                if (target.Members.Any())
                    reactions.Add(new ProposedReaction(reaction, reactor, target, timing));
            }
            return reactions.ToArray();
        };

    protected static Target GetReactionTarget(Member possessor, Member reactor, IDictionary<int, Member> members, CardReactionSequence action, Member effectSource, Target effectTarget)
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

    protected static bool ReactionIsApplicable(Member possessor, bool canReactToReactions, EffectResolved effect, int triggerCount)
    {
        if (effect.IsReaction && !canReactToReactions)
            return false;
        if (triggerCount == 0)
            return false;
        return true;
    }
}

public class ClonedReactiveEffect : ReactiveEffectV2Base
{
    public ClonedReactiveEffect(ReactionTimingWindow timing, bool onlyReactDuringCardPhases, TemporalStateMetadata metadata, 
        Func<EffectResolved, ProposedReaction[]> createMaybeEffects) 
            : base(timing, onlyReactDuringCardPhases, metadata, createMaybeEffects) {}
}
