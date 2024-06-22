using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectReactWith : Effect
{
    private static Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, int>>> Conditions = new Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, int>>>
    {
        { ReactionConditionType.OnGainedTaunt, ctx => effect => IncreasedBy(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Taunt])) },
        { ReactionConditionType.WhenHas10CardsOrMoreInHand, ctx => effect => BoolToInt(ctx.Possessor.IsConscious() && effect.CardZones.HandZone.Count >= 10) },
        { ReactionConditionType.OnArchetypeCardDrawn, ctx => effect => ctx.Actor.IsConscious() ? effect.DrawnCards.Count(x => x.Archetypes.Contains(ctx.ReactionEffectScope)) : 0 },
        { ReactionConditionType.OnTeamCardCycled, ctx => effect => ctx.Actor.IsConscious() ? effect.CycledCards.Length : 0 },
        { ReactionConditionType.OnCardPlayed, ctx => effect => BoolToInt(ctx.Actor.IsConscious() && effect.OriginatingCard.IsPresentAnd(c => c.Owner.Id == ctx.Possessor.Id) && effect.IsFirstBattleEffectOfChosenTarget) },
        { ReactionConditionType.OnNonQuickCardPlayed, ctx => effect => BoolToInt(ctx.Actor.IsConscious() && effect.OriginatingCard.IsPresentAnd(c =>  c.Owner.Id == ctx.Possessor.Id && !c.IsQuick) && effect.IsFirstBattleEffectOfChosenTarget) },
        { ReactionConditionType.WhenAttacked, ctx => effect => BoolToInt(
            ctx.Actor.IsConscious()
            && new [] {EffectType.AttackFormula, EffectType.MagicAttackFormula, EffectType.TrueDamageAttackFormula}.Contains(effect.EffectData.EffectType)
            && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id)) },
        { ReactionConditionType.WhenBloodied, ctx => effect => BoolToInt(
            ctx.Actor.IsConscious() && !effect.BattleBefore.Members[ctx.Possessor.Id].IsBloodied() && effect.BattleAfter.Members[ctx.Possessor.Id].IsBloodied()) },
        {ReactionConditionType.WhenVulnerabled, ctx => effect => BoolToInt(
            ctx.Actor.IsConscious() 
            && effect.EffectData.EffectScope.Value == "Vulnerable"
            && effect.BattleBefore.Members[ctx.Possessor.Id].State[TemporalStatType.Vulnerable] < effect.BattleAfter.Members[ctx.Possessor.Id].State[TemporalStatType.Vulnerable] 
            && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id)) },
        { ReactionConditionType.WhenShieldBroken, ctx => effect => BoolToInt(ctx.Possessor.IsConscious() && WentToZero(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Shield]))) },
        { ReactionConditionType.WhenShielded, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Shield])) : 0 },
        { ReactionConditionType.WhenDamagedHp, ctx => effect => ctx.Actor.IsConscious() ? DecreasedBy(effect.Select(ctx.Possessor, m => m.State.Hp)) : 0 },
        { ReactionConditionType.WhenDamagedShield, ctx => effect => ctx.Actor.IsConscious() ? DecreasedBy(effect.Select(ctx.Possessor, m => m.State.Shield)) : 0 },
        { ReactionConditionType.WhenDamaged, ctx => effect => ctx.Actor.IsConscious() ? DecreasedBy(effect.Select(ctx.Possessor, m => m.State.Hp + m.State.Shield)) : 0 },
        { ReactionConditionType.WhenBlinded, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Blind])) : 0 },
        { ReactionConditionType.WhenGainedPrimaryResource, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State.PrimaryResourceAmount)) : 0 },
        { ReactionConditionType.WhenShieldMaxed, ctx => effect 
            => BoolToInt(ctx.Possessor.IsConscious() && BecameTrue(effect.Select(ctx.Possessor, x => x.State.Shield == x.State.MaxShield )))},
        { ReactionConditionType.WhenHealthMaxed, ctx => effect 
            => BoolToInt(ctx.Possessor.IsConscious() && BecameTrue(effect.Select(ctx.Possessor, x => x.State.Hp == x.State.MaxHp )))},
        { ReactionConditionType.OnCausedShieldGain, ctx => effect 
            => PossessorCaused(ctx, effect) ? IncreasedBy(effect.SelectSum(x => x.State[TemporalStatType.Shield])) : 0},
        { ReactionConditionType.OnCausedStun, ctx => effect 
            => PossessorCaused(ctx, effect) ? IncreasedBy(effect.SelectSum(x => x.State[TemporalStatType.Stun] + x.State[TemporalStatType.Disabled])) : 0 },
        { ReactionConditionType.OnCausedAffliction, ctx => effect 
            => PossessorCaused(ctx, effect) ? IncreasedBy(effect.SelectSum(x => x.State.StatusesOfType[StatusTag.DamageOverTime])) : 0 },
        { ReactionConditionType.OnSlay, ctx => effect
            => PossessorCaused(ctx, effect) ? IncreasedBy(effect.SelectSum(x => x.IsUnconscious() ? 1 : 0)) : 0 },
        { ReactionConditionType.OnCausedHeal, ctx => effect
            => PossessorCaused(ctx, effect) ? IncreasedBy(effect.SelectSum(x => x.State.Hp)) : 0 },
        { ReactionConditionType.OnCausedBloodied, ctx => effect =>
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return 0;

                var targetMembers = effect.Target.Members;
                var becameBloodied = 0;
                foreach (var m in targetMembers)
                    if (!effect.BattleBefore.Members[m.Id].IsBloodied() && effect.BattleAfter.Members[m.Id].IsBloodied())
                        becameBloodied++;
                return becameBloodied;
            }
        },
        { ReactionConditionType.WhenKilled, ctx => effect => BoolToInt(ctx.Possessor.IsUnconscious() && (ctx.Possessor.Id == ctx.Actor.Id || ctx.Actor.IsConscious())) },
        { ReactionConditionType.OnDamageDealt, ctx => effect
                =>
                {
                    if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                        return 0;

                    return Mathf.Clamp(effect.BattleBefore.TargetMembers(effect.Target).Sum(x => x.State.Hp + x.State.Shield) -
                           effect.BattleAfter.TargetMembers(effect.Target).Sum(x => x.State.Hp + x.State.Shield), 0, 999);
                }
        },
        { ReactionConditionType.OnHpDamageDealt, ctx => effect 
            => 
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return 0;

                var beforeHp = effect.BattleBefore.TargetMembers(effect.Target).Sum(x => x.State.Hp);
                var afterHp = effect.BattleAfter.TargetMembers(effect.Target).Sum(x => x.State.Hp);
                return Mathf.Clamp(beforeHp - afterHp, 0, 999);
            }
        },
        { ReactionConditionType.OnClipUsed, ctx => effect =>
            {
                if (ctx.Actor.IsUnconscious())
                    return 0;
                if (effect.EffectData.EffectType == EffectType.Reload && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id))
                    return 1;
                if (!effect.Source.Equals(ctx.Possessor))
                    return 0;
                return BoolToInt(ctx.Possessor.State.PrimaryResourceAmount == 0 && effect.BattleBefore.Members[ctx.Possessor.Id].State.PrimaryResourceAmount > 0);
            }
        },
        { ReactionConditionType.OnTagCardPlayed, ctx => effect => BoolToInt(IsRelevant(ReactionConditionType.OnTagCardPlayed, effect, ctx) 
           && effect.IsFirstBattleEffectOfChosenTarget && effect.OriginatingCard.IsPresentAnd(x => x.Type.Tags.Contains(ctx.ReactionEffectScope.EnumVal<CardTag>())))},
        { ReactionConditionType.OnArchetypeCardPlayed, ctx => effect => BoolToInt(IsRelevant(ReactionConditionType.OnArchetypeCardPlayed, effect, ctx)
           && effect.IsFirstBattleEffectOfChosenTarget && effect.OriginatingCard.IsPresentAnd(x => x.Archetypes.Contains(ctx.ReactionEffectScope)))},
        { ReactionConditionType.OnDodged, ctx => effect => BoolToInt(ctx.Possessor.IsConscious() 
           && effect.Preventions.IsDodging(ctx.Possessor) 
           && Decreased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Dodge])))},
        { ReactionConditionType.OnAegised, ctx => effect => BoolToInt(ctx.Possessor.IsConscious() 
           && effect.Preventions.IsAegising(ctx.Possessor) 
           && Decreased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Aegis])))},
        { ReactionConditionType.WhenNearDeath, ctx => effect => BoolToInt(ctx.Actor.IsConscious() && ctx.Possessor.CurrentHp() == 1) },
        { ReactionConditionType.WhenAllyDeath, ctx => effect => BoolToInt(ctx.Actor.IsConscious() 
            && effect.BattleBefore.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious()) 
              > effect.BattleAfter.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious())) },
        { ReactionConditionType.WhenNonSelfAllyBloodied, ctx => effect => ctx.Actor.IsConscious() 
            ? IncreasedBy(effect.Select(b => b.NonSelfAllies(ctx.Actor.Id).Count(x => x.Value.IsBloodied()))) : 0 },
        { ReactionConditionType.WhenNonSelfAllyHpDamaged, ctx => effect => BoolToInt(ctx.Actor.IsConscious() 
            && Decreased(effect.Select(b => b.NonSelfAllies(ctx.Actor.Id).Sum(x => x.Value.State.Hp)))) },
        { ReactionConditionType.WhenNonSelfAllyHpDamagedButNotKilled, ctx => effect => BoolToInt(ctx.Actor.IsConscious()
            && Decreased(Logged(effect.Select(b => b.NonSelfConsciousAllies(ctx.Actor.Id).Sum(x => x.Value.State.Hp))))) },
        { ReactionConditionType.WhenAfflicted, ctx => effect => ctx.Actor.IsConscious() 
            ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State.StatusesOfType[StatusTag.DamageOverTime])) : 0 },
        { ReactionConditionType.OnAppliedMark, ctx => effect => ctx.Actor.IsConscious() 
            ? IncreasedBy(effect.Select(b => b.TargetMembers(effect.Target).Sum(x => x.State[TemporalStatType.Marked]))) : 0 },
        { ReactionConditionType.OnStealthed, ctx => effect => BoolToInt(ctx.Actor.IsConscious() 
            && !effect.BattleBefore.Members[ctx.Possessor.Id].IsStealthed()
            && Increased(effect.Select(ctx.Actor, m => m.State[TemporalStatType.Stealth]))) },
        { ReactionConditionType.WhenEnemyPrimaryStatBuffed, ctx => effect =>
            {
                if (!ctx.Actor.IsConscious())
                    return 0;
                if (effect.Target.Members.All(x => x.TeamType == ctx.Possessor.TeamType))
                    return 0;
                var membersAffected = effect.BattleAfter.Members.Count(after =>
                {
                    var before = effect.BattleBefore.Members.Select(x => x.Value).FirstOrDefault(before => before.Id == after.Key);
                    return before != null 
                           && after.Value.TeamType != ctx.Possessor.TeamType 
                           && after.Value.State.Stats[after.Value.State.PrimaryStat] > before.State.Stats[before.State.PrimaryStat];
                });
                if (membersAffected > 0)
                    return membersAffected;
                if (effect.EffectData.EffectType == EffectType.AdjustPrimaryStatForEveryCardCycledAndInHand
                    && effect.EffectData.FloatAmount > 0)
                    return 1;
                return 0;
            }},
        { ReactionConditionType.WhenAllyVulnerable, ctx => effect 
            => BoolToInt(ctx.Actor.IsConscious() && effect.EffectData.EffectScope.Value == "Vulnerable" && effect.Target.Members.Any(x => x.Id != ctx.Possessor.Id && x.TeamType == ctx.Possessor.TeamType)) },
        { ReactionConditionType.OnResourcesLost, ctx => effect 
            => ctx.Actor.IsConscious() && effect.Source.Id != ctx.Actor.Id ? Mathf.Clamp(effect.BattleBefore.Members[ctx.Actor.Id].State.PrimaryResourceAmount - effect.BattleAfter.Members[ctx.Actor.Id].State.PrimaryResourceAmount, 0, 999) : 0 },
        { ReactionConditionType.WhenEnemyShielded, ctx => effect 
            => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && effect.BattleBefore.Members[x.Id].State.Shield < effect.BattleAfter.Members[x.Id].State.Shield) : 0 },
        { ReactionConditionType.WhenAllyBloodied, ctx => effect 
            => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => 
                x.TeamType == ctx.Actor.TeamType 
                && x.Id != ctx.Actor.Id 
                && !effect.BattleBefore.Members[x.Id].IsBloodied()
                && effect.BattleAfter.Members[x.Id].IsBloodied()) : 0 },
        { ReactionConditionType.WhenNonSelfAllyBloodiedButNotKilled, ctx => effect 
            => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => 
                x.IsConscious() && x.TeamType == ctx.Actor.TeamType 
                && x.Id != ctx.Actor.Id 
                && !effect.BattleBefore.Members[x.Id].IsBloodied()
                && effect.BattleAfter.Members[x.Id].IsBloodied()) : 0 },
        { ReactionConditionType.WhenStunned, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Stun])) : 0 },
        { ReactionConditionType.WhenMarked, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Marked])) : 0 },
        { ReactionConditionType.WhenStatsReduced, ctx => effect => ctx.Possessor.IsConscious() && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) ? DecreasedBy(effect.Select(ctx.Possessor, m => Mathf.RoundToInt(m.State[m.State.PrimaryStat]))) : 0 },
        { ReactionConditionType.WhenEnemyStealthed, ctx => effect => ctx.Actor.IsConscious() && effect.EffectData.EffectType == EffectType.EnterStealth ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType) : 0 },
        { ReactionConditionType.WhenEnemyTaunted, ctx => effect => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Taunt]))) : 0 },
        { ReactionConditionType.WhenEnemyGainedDodge, ctx => effect => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Dodge]))) : 0 },
        { ReactionConditionType.WhenEnemyGainedAegis, ctx => effect => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Aegis]))) : 0 },
        { ReactionConditionType.WhenEnemyGainedLifesteal, ctx => effect => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Lifesteal]))) : 0 },
        { ReactionConditionType.WhenEnemyGainedResources, ctx => effect => ctx.Actor.IsConscious() ? effect.Target.Members.Count(x => x.TeamType != ctx.Actor.TeamType && effect.BattleBefore.Members[x.Id].State.PrimaryResourceAmount < effect.BattleAfter.Members[x.Id].State.PrimaryResourceAmount) : 0 },
        { ReactionConditionType.WhenDotted, ctx => effect => ctx.Possessor.IsConscious() ? IncreasedBy(effect.Select(ctx.Possessor, m => m.State.StatusesOfType[StatusTag.DamageOverTime])) : 0 },
    };

    private static int BoolToInt(bool value) => value ? 1 : 0;

    private static bool IsRelevant(ReactionConditionType type, EffectResolved effect, ReactionConditionContext ctx)
        => (effect.EffectData.EffectType != EffectType.ReactWithCard || effect.EffectData.ReactionConditionType != type)
           && (effect.EffectData.EffectType != EffectType.ReactWithEffect || effect.EffectData.ReactionConditionType != type)
           && ctx.Actor.IsConscious();
    
    private static bool PossessorCaused(ReactionConditionContext ctx, EffectResolved effect) 
        => Equals(ctx.Possessor, effect.Source) && !ctx.Actor.IsUnconscious();

    private static bool BecameTrue(bool[] values) => !values.First() && values.Last();
    private static bool WentToZero(int[] values) => values.First() > 0 && values.Last() == 0;
    private static bool Decreased(params int[] values) => values.Last() < values.First();
    
    private static int DecreasedBy(params int[] values) => Mathf.Clamp(values.First() - values.Last(), 0, 999);
    private static bool Increased(params int[] values) => values.Last() > values.First();
    private static int IncreasedBy(params int[] values) => Mathf.Clamp(values.Last() - values.First(), 0, 999);

    private static T[] Logged<T>(T[] values)
    {
        Debug.Log(string.Join(", ", values.Select(v => v.ToString())));
        return values;
    }
    
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly string _maxDurationFormula;
    private readonly string _reactionEffectContext;
    private readonly ReactionTimingWindow _timing;
    private readonly bool _onlyReactDuringCardPhases;
    private readonly StatusDetail _status;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly Maybe<CardReactionSequence> _reactionEffect;
    private readonly Maybe<ReactionCardType> _reactionCard;
    private readonly ReactionConditionType _conditionType;
    private readonly int _maxUsesPerTurn;
    private readonly Func<ReactionConditionContext, Func<EffectResolved, int>> _conditionBuilder;

    public EffectReactWith(bool isDebuff, int numberOfUses, string maxDurationFormula, StatusDetail status, 
        string reactionBonusEffectContext, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        ReactionConditionType conditionType, ReactionCardType reactionCard)
            : this(isDebuff, numberOfUses, maxDurationFormula, status, reactionBonusEffectContext, timing, onlyReactDuringCardPhases, triggerScope, conditionType, Maybe<CardReactionSequence>.Missing(), 
                new Maybe<ReactionCardType>(reactionCard, true)) {}
    
    public EffectReactWith(bool isDebuff, int numberOfUses, string maxDurationFormula, StatusDetail status, 
        string reactionBonusEffectContext, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        ReactionConditionType conditionType, CardReactionSequence reactionEffect)
        : this(isDebuff, numberOfUses, maxDurationFormula, status, reactionBonusEffectContext, timing, onlyReactDuringCardPhases, triggerScope, conditionType, new Maybe<CardReactionSequence>(reactionEffect, true), 
            Maybe<ReactionCardType>.Missing()) {}
    
    public EffectReactWith(bool isDebuff, int numberOfUses, string maxDurationFormula, StatusDetail status, 
        string reactionBonusEffectContext, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        ReactionConditionType conditionType, Maybe<CardReactionSequence> reactionEffect, Maybe<ReactionCardType> reactionCard,
        int maxUsesPerTurn = -1)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationFormula = maxDurationFormula;
        _status = status;
        _triggerScope = triggerScope;
        _reactionEffect = reactionEffect;
        _reactionCard = reactionCard;
        _conditionType = conditionType;
        _conditionBuilder = Conditions.VerboseGetValue(conditionType, conditionType.ToString());
        _reactionEffectContext = reactionBonusEffectContext;
        _timing = timing;
        _onlyReactDuringCardPhases = onlyReactDuringCardPhases;
        _maxUsesPerTurn = maxUsesPerTurn;
        if (reactionCard.IsMissing && reactionEffect.IsMissing)
            Log.Info($"React With neither has an Effect nor a Card.");
    }
    
    public void Apply(EffectContext ctx)
    {
        if (_reactionCard.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                var reactionConditionContext = new ReactionConditionContext { ReactionEffectScope = _reactionEffectContext };
                if (!ctx.BattleMembers.ContainsKey(m.MemberId))
                    return;
                
                reactionConditionContext.Possessor = ctx.BattleMembers.VerboseGetValue(m.MemberId, nameof(ctx.BattleMembers));
                reactionConditionContext.Actor = _reactionCard.Value.ActionSequence.Reactor == ReactiveMember.Originator ? ctx.Source : reactionConditionContext.Possessor;
                m.AddReactiveState(new ReactWithCard(_isDebuff, _numberOfUses, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m, _maxDurationFormula, ctx.XPaidAmount, ctx.ScopedData),
                    _status, _timing, _onlyReactDuringCardPhases, _triggerScope, ctx.BattleMembers, m.MemberId, ctx.Source, _reactionCard.Value,
                    _conditionBuilder(reactionConditionContext)));
                #if UNITY_EDITOR
                DevLog.Write($"Applied React With Card {_conditionType} to {m.NameTerm.ToEnglish()}");
                #endif
            });
        else if (_reactionEffect.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                var reactionConditionContext = new ReactionConditionContext { ReactionEffectScope = _reactionEffectContext };
                if (!ctx.BattleMembers.ContainsKey(m.MemberId))
                    return;

                reactionConditionContext.Possessor = ctx.BattleMembers.VerboseGetValue(m.MemberId, nameof(ctx.BattleMembers));
                reactionConditionContext.Actor = _reactionEffect.Value.Reactor == ReactiveMember.Originator ? ctx.Source : reactionConditionContext.Possessor;
                m.AddReactiveState(new ReactWithEffect(_isDebuff, _numberOfUses, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m, _maxDurationFormula, ctx.XPaidAmount, ctx.ScopedData), 
                    _status, _timing, _onlyReactDuringCardPhases, _triggerScope, ctx.BattleMembers, m.MemberId, ctx.Source, _reactionEffect.Value, _maxUsesPerTurn,
                    _conditionBuilder(reactionConditionContext)));
                #if UNITY_EDITOR
                DevLog.Write($"Applied React With Effect {_conditionType} to {m.NameTerm.ToEnglish()}");
                #endif
            });
    }
}

public sealed class ReactWithEffect : ReactiveEffectV2Base
{
    public ReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, CardReactionSequence reaction, int maxUsesPerTurn, Func<EffectResolved, int> condition)
            : base(originator.Id, isDebuff, maxDurationTurns, numberOfUses, status, timing, onlyReactDuringCardPhases, maxUsesPerTurn, CreateMaybeEffects(allMembers, possessingMemberId, originator, true, reaction, timing,
                effect =>
                {
                    var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                    var triggerCount = condition(effect);
                    #if UNITY_EDITOR
                    DevLog.Write($"{status.Tag} Reaction - Member {possessingMemberId}- Is In Trigger Scope: {isInTriggerScope}. Condition Met: {triggerCount} times");
                    #endif
                    return effect.WasApplied && isInTriggerScope ? triggerCount : 0;
                })) {}
}

public sealed class ReactWithCard : ReactiveEffectV2Base
{
    public ReactWithCard(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction, Func<EffectResolved, int> condition)
        : base(originator.Id, isDebuff, maxDurationTurns, numberOfUses, status, timing, onlyReactDuringCardPhases, CreateMaybeEffects(allMembers, possessingMemberId, originator, true, reaction, timing,
            effect =>
            {
                var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                var triggerCount = condition(effect);
                #if UNITY_EDITOR
                DevLog.Write($"{status.Tag} Reaction - Member {possessingMemberId}- Is In Trigger Scope: {isInTriggerScope}. Condition Met: {triggerCount} times");
                #endif
                return effect.WasApplied && isInTriggerScope ? triggerCount : 0;
            })) {}
}

