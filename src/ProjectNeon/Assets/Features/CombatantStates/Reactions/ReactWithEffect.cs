using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectReactWith : Effect
{
    private static Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, bool>>> Conditions = new Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, bool>>>
    {
        { ReactionConditionType.WhenHas10CardsOrMoreInHand, ctx => effect => ctx.Possessor.IsConscious() && effect.CardZones.HandZone.Count >= 10 },
        { ReactionConditionType.OnTeamCardCycled, ctx => effect => ctx.Actor.IsConscious() && effect.CycledCard.IsPresent },
        { ReactionConditionType.OnCardPlayed, ctx => effect => ctx.Actor.IsConscious() && effect.Card.IsPresentAnd(c => c.Owner.Id == ctx.Possessor.Id) && effect.IsFirstBattleEffectOfChosenTarget },
        { ReactionConditionType.OnNonQuickCardPlayed, ctx => effect => ctx.Actor.IsConscious() && effect.Card.IsPresentAnd(c =>  c.Owner.Id == ctx.Possessor.Id && !c.IsQuick) && effect.IsFirstBattleEffectOfChosenTarget },
        { ReactionConditionType.WhenAttacked, ctx => effect => 
            ctx.Actor.IsConscious()
            && new [] {EffectType.AttackFormula, EffectType.MagicAttackFormula, EffectType.TrueDamageAttackFormula}.Contains(effect.EffectData.EffectType)
            && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) },
        { ReactionConditionType.WhenBloodied, ctx => effect => 
            ctx.Actor.IsConscious() && !effect.BattleBefore.Members[ctx.Possessor.Id].IsBloodied() && effect.BattleAfter.Members[ctx.Possessor.Id].IsBloodied() },
        {ReactionConditionType.WhenVulnerabled, ctx => effect 
            => ctx.Actor.IsConscious() 
            && effect.EffectData.EffectScope.Value == "Vulnerable"
            && effect.BattleBefore.Members[ctx.Possessor.Id].State[TemporalStatType.Vulnerable] < effect.BattleAfter.Members[ctx.Possessor.Id].State[TemporalStatType.Vulnerable] 
            && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) },
        { ReactionConditionType.WhenShieldBroken, ctx => effect => ctx.Possessor.IsConscious() && WentToZero(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Shield])) },
        { ReactionConditionType.WhenShielded, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Shield])) },
        { ReactionConditionType.WhenDamagedHp, ctx => effect => ctx.Actor.IsConscious() && Decreased(effect.Select(ctx.Possessor, m => m.State.Hp))},
        { ReactionConditionType.WhenDamagedShield, ctx => effect => ctx.Actor.IsConscious() && Decreased(effect.Select(ctx.Possessor, m => m.State.Shield))},
        { ReactionConditionType.WhenDamaged, ctx => effect => ctx.Actor.IsConscious() && Decreased(effect.Select(ctx.Possessor, m => m.State.Hp + m.State.Shield))},
        { ReactionConditionType.WhenBlinded, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Blind])) },
        { ReactionConditionType.WhenGainedPrimaryResource, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State.PrimaryResourceAmount)) },
        { ReactionConditionType.WhenShieldMaxed, ctx => effect 
            => ctx.Possessor.IsConscious() && BecameTrue(effect.Select(ctx.Possessor, x => x.State.Shield == x.State.MaxShield ))},
        { ReactionConditionType.OnCausedShieldGain, ctx => effect 
            => PossessorCaused(ctx, effect) && Increased(effect.SelectSum(x => x.State[TemporalStatType.Shield]))},
        { ReactionConditionType.OnCausedStun, ctx => effect 
            => PossessorCaused(ctx, effect) && Increased(effect.SelectSum(x => x.State[TemporalStatType.Stun] + x.State[TemporalStatType.Disabled]))},
        { ReactionConditionType.OnCausedAffliction, ctx => effect 
            => PossessorCaused(ctx, effect) && Increased(effect.SelectSum(x => x.State.StatusesOfType[StatusTag.DamageOverTime])) },
        { ReactionConditionType.OnSlay, ctx => effect
            => PossessorCaused(ctx, effect) && Increased(effect.SelectSum(x => x.IsUnconscious() ? 1 : 0)) },
        { ReactionConditionType.OnCausedHeal, ctx => effect
            => PossessorCaused(ctx, effect) && Increased(effect.SelectSum(x => x.State.Hp)) },
        { ReactionConditionType.OnCausedBloodied, ctx => effect =>
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return false;

                var targetMembers = effect.Target.Members;
                foreach (var m in targetMembers)
                {
                    var becameBloodied = !effect.BattleBefore.Members[m.Id].IsBloodied() && effect.BattleAfter.Members[m.Id].IsBloodied();
                    if (becameBloodied)
                        return true;
                }
                return false;
            }
        },
        { ReactionConditionType.WhenKilled, ctx => effect => ctx.Possessor.IsUnconscious() && (ctx.Possessor.Id == ctx.Actor.Id || ctx.Actor.IsConscious()) },
        { ReactionConditionType.OnDamageDealt, ctx => effect
                =>
                {
                    if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                        return false;

                    return effect.BattleBefore.TargetMembers(effect.Target).Sum(x => x.State.Hp + x.State.Shield) >
                           effect.BattleAfter.TargetMembers(effect.Target).Sum(x => x.State.Hp + x.State.Shield);
                }
        },
        { ReactionConditionType.OnHpDamageDealt, ctx => effect 
            => 
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return false;

                var beforeHp = effect.BattleBefore.TargetMembers(effect.Target).Sum(x => x.State.Hp);
                var afterHp = effect.BattleAfter.TargetMembers(effect.Target).Sum(x => x.State.Hp);
                return beforeHp > afterHp;
            }
        },
        { ReactionConditionType.OnClipUsed, ctx => effect =>
            {
                if (ctx.Actor.IsUnconscious())
                    return false;
                if (effect.EffectData.EffectType == EffectType.Reload && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id))
                    return true;
                if (!effect.Source.Equals(ctx.Possessor))
                    return false;
                return ctx.Possessor.State.PrimaryResourceAmount == 0 && effect.BattleBefore.Members[ctx.Possessor.Id].State.PrimaryResourceAmount > 0;
            }
        },
        { ReactionConditionType.OnTagCardPlayed, ctx => effect => IsRelevant(ReactionConditionType.OnTagCardPlayed, effect, ctx) 
           && effect.IsFirstBattleEffectOfChosenTarget && effect.Card.IsPresentAnd(x => x.Type.Tags.Contains(ctx.ReactionEffectScope.EnumVal<CardTag>()))},
        { ReactionConditionType.OnArchetypeCardPlayed, ctx => effect => IsRelevant(ReactionConditionType.OnArchetypeCardPlayed, effect, ctx)
           && effect.IsFirstBattleEffectOfChosenTarget && effect.Card.IsPresentAnd(x => x.Archetypes.Contains(ctx.ReactionEffectScope))},
        { ReactionConditionType.OnDodged, ctx => effect => ctx.Possessor.IsConscious() 
           && effect.Preventions.IsDodging(ctx.Possessor) 
           && Decreased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Dodge]))},
        { ReactionConditionType.OnAegised, ctx => effect => ctx.Possessor.IsConscious() 
           && effect.Preventions.IsAegising(ctx.Possessor) 
           && Decreased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Aegis]))},
        { ReactionConditionType.WhenNearDeath, ctx => effect => ctx.Actor.IsConscious() && ctx.Possessor.CurrentHp() == 1 },
        { ReactionConditionType.WhenAllyDeath, ctx => effect => ctx.Actor.IsConscious() 
            && effect.BattleBefore.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious()) 
              > effect.BattleAfter.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious()) },
        { ReactionConditionType.WhenNonSelfAllyBloodied, ctx => effect => ctx.Actor.IsConscious() 
            && Increased(effect.Select(b => b.NonSelfAllies(ctx.Actor.Id).Count(x => x.Value.IsBloodied()))) },
        { ReactionConditionType.WhenNonSelfAllyHpDamaged, ctx => effect => ctx.Actor.IsConscious() 
            && Decreased(effect.Select(b => b.NonSelfAllies(ctx.Actor.Id).Sum(x => x.Value.State.Hp))) },
        { ReactionConditionType.WhenNonSelfAllyHpDamagedButNotKilled, ctx => effect => ctx.Actor.IsConscious()
            && Decreased(Logged(effect.Select(b => b.NonSelfConsciousAllies(ctx.Actor.Id).Sum(x => x.Value.State.Hp)))) },
        { ReactionConditionType.WhenAfflicted, ctx => effect => ctx.Actor.IsConscious() 
            && Increased(effect.Select(ctx.Possessor, m => m.State.StatusesOfType[StatusTag.DamageOverTime])) },
        { ReactionConditionType.OnAppliedMark, ctx => effect => ctx.Actor.IsConscious() 
            && Increased(effect.Select(b => b.TargetMembers(effect.Target).Sum(x => x.State[TemporalStatType.Marked]))) },
        { ReactionConditionType.OnStealthed, ctx => effect => ctx.Actor.IsConscious() 
            && !effect.BattleBefore.Members[ctx.Possessor.Id].IsStealthed()
            && Increased(effect.Select(ctx.Actor, m => m.State[TemporalStatType.Stealth])) },
        { ReactionConditionType.WhenEnemyPrimaryStatBuffed, ctx => effect =>
            {
                if (!ctx.Actor.IsConscious())
                    return false;
                if (effect.Target.Members.All(x => x.TeamType == ctx.Possessor.TeamType))
                    return false;
                if (effect.BattleAfter.Members.Any(after =>
                    {
                        var before = effect.BattleBefore.Members.Select(x => x.Value).FirstOrDefault(before => before.Id == after.Key);
                        return before != null 
                               && after.Value.TeamType != ctx.Possessor.TeamType 
                               && after.Value.State.Stats[after.Value.State.PrimaryStat] > before.State.Stats[before.State.PrimaryStat];
                    }))
                    return true;
                if (effect.EffectData.EffectType == EffectType.AdjustPrimaryStatForEveryCardCycledAndInHand
                    && effect.EffectData.FloatAmount > 0)
                    return true;
                return false;
            }},
        { ReactionConditionType.WhenAllyVulnerable, ctx => effect 
            => ctx.Actor.IsConscious() && effect.EffectData.EffectScope.Value == "Vulnerable" && effect.Target.Members.Any(x => x.Id != ctx.Possessor.Id && x.TeamType == ctx.Possessor.TeamType) },
        { ReactionConditionType.OnResourcesLost, ctx => effect 
            => ctx.Actor.IsConscious() && effect.Source.Id != ctx.Actor.Id && effect.BattleBefore.Members[ctx.Actor.Id].State.PrimaryResourceAmount > effect.BattleAfter.Members[ctx.Actor.Id].State.PrimaryResourceAmount },
        { ReactionConditionType.WhenEnemyShielded, ctx => effect 
            => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && effect.BattleBefore.Members[x.Id].State.Shield < effect.BattleAfter.Members[x.Id].State.Shield) },
        { ReactionConditionType.WhenAllyBloodied, ctx => effect 
            => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => 
                x.TeamType == ctx.Actor.TeamType 
                && x.Id != ctx.Actor.Id 
                && !effect.BattleBefore.Members[x.Id].IsBloodied()
                && effect.BattleAfter.Members[x.Id].IsBloodied()) },
        { ReactionConditionType.WhenNonSelfAllyBloodiedButNotKilled, ctx => effect 
            => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => 
                x.IsConscious() && x.TeamType == ctx.Actor.TeamType 
                && x.Id != ctx.Actor.Id 
                && !effect.BattleBefore.Members[x.Id].IsBloodied()
                && effect.BattleAfter.Members[x.Id].IsBloodied()) },
        { ReactionConditionType.WhenStunned, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Stun])) },
        { ReactionConditionType.WhenMarked, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Marked])) },
        { ReactionConditionType.WhenStatsReduced, ctx => effect => ctx.Possessor.IsConscious() && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) && Decreased(effect.Select(ctx.Possessor, m => Mathf.RoundToInt(m.State[m.State.PrimaryStat]))) },
        { ReactionConditionType.WhenEnemyStealthed, ctx => effect => ctx.Actor.IsConscious() && effect.EffectData.EffectType == EffectType.EnterStealth && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType) },
        { ReactionConditionType.WhenEnemyTaunted, ctx => effect => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Taunt]))) },
        { ReactionConditionType.WhenEnemyGainedDodge, ctx => effect => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Dodge]))) },
        { ReactionConditionType.WhenEnemyGainedAegis, ctx => effect => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Aegis]))) },
        { ReactionConditionType.WhenEnemyGainedLifesteal, ctx => effect => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && Increased(effect.Select(ctx.Possessor, m => m.State[TemporalStatType.Lifesteal]))) },
        { ReactionConditionType.WhenEnemyGainedResources, ctx => effect => ctx.Actor.IsConscious() && effect.Target.Members.Any(x => x.TeamType != ctx.Actor.TeamType && effect.BattleBefore.Members[x.Id].State.PrimaryResourceAmount < effect.BattleAfter.Members[x.Id].State.PrimaryResourceAmount) },
        { ReactionConditionType.WhenDotted, ctx => effect => ctx.Possessor.IsConscious() && Increased(effect.Select(ctx.Possessor, m => m.State.StatusesOfType[StatusTag.DamageOverTime])) },
    };

    private static bool IsRelevant(ReactionConditionType type, EffectResolved effect, ReactionConditionContext ctx)
        => (effect.EffectData.EffectType != EffectType.ReactWithCard || effect.EffectData.ReactionConditionType != type)
           && (effect.EffectData.EffectType != EffectType.ReactWithEffect || effect.EffectData.ReactionConditionType != type)
           && ctx.Actor.IsConscious();
    
    private static bool PossessorCaused(ReactionConditionContext ctx, EffectResolved effect) 
        => Equals(ctx.Possessor, effect.Source) && !ctx.Actor.IsUnconscious();

    private static bool BecameTrue(bool[] values) => !values.First() && values.Last();
    private static bool WentToZero(int[] values) => values.First() > 0 && values.Last() == 0;
    private static bool Decreased(params int[] values) => values.Last() < values.First();
    private static bool Increased(params int[] values) => values.Last() > values.First();

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
    private readonly Func<ReactionConditionContext, Func<EffectResolved, bool>> _conditionBuilder;

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
                DevLog.Write($"Applied React With Card {_conditionType} to {m.NameTerm.ToEnglish()}");
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
                DevLog.Write($"Applied React With Effect {_conditionType} to {m.NameTerm.ToEnglish()}");
            });
    }
}

public sealed class ReactWithEffect : ReactiveEffectV2Base
{
    public ReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, CardReactionSequence reaction, int maxUsesPerTurn, Func<EffectResolved, bool> condition)
            : base(originator.Id, isDebuff, maxDurationTurns, numberOfUses, status, timing, onlyReactDuringCardPhases, maxUsesPerTurn, CreateMaybeEffect(allMembers, possessingMemberId, originator, true, reaction, timing,
                effect =>
                {
                    var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                    var conditionMet = condition(effect);
                    DevLog.Write($"{status.Tag} Reaction - Member {possessingMemberId}- Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                    return effect.WasApplied && isInTriggerScope && conditionMet;
                })) {}
}

public sealed class ReactWithCard : ReactiveEffectV2Base
{
    public ReactWithCard(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactionTimingWindow timing, bool onlyReactDuringCardPhases, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction, Func<EffectResolved, bool> condition)
        : base(originator.Id, isDebuff, maxDurationTurns, numberOfUses, status, timing, onlyReactDuringCardPhases, CreateMaybeEffect(allMembers, possessingMemberId, originator, true, reaction, timing,
            effect =>
            {
                var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                var conditionMet = condition(effect);
                DevLog.Write($"{status.Tag} Reaction - Member {possessingMemberId}- Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                return effect.WasApplied && isInTriggerScope && conditionMet;
            })) {}
}

