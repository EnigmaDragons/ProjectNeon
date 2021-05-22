using System;
using System.Collections.Generic;
using System.Linq;

public class EffectReactWith : Effect
{
    private static Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, bool>>> Conditions = new Dictionary<ReactionConditionType, Func<ReactionConditionContext, Func<EffectResolved, bool>>>
    {    
        { ReactionConditionType.OnAttacked, ctx => effect => 
            ctx.Actor.IsConscious() && (effect.EffectData.EffectType == EffectType.AttackFormula) && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) },
        { ReactionConditionType.OnMagicAttacked, ctx => effect => 
            ctx.Actor.IsConscious() && effect.EffectData.EffectType == EffectType.MagicAttackFormula && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) },
        { ReactionConditionType.OnBloodied, ctx => effect => 
            ctx.Actor.IsConscious() && !effect.BattleBefore.Members[ctx.Possessor.Id].IsBloodied() && effect.BattleAfter.Members[ctx.Possessor.Id].IsBloodied() },
        {ReactionConditionType.OnVulnerable, ctx => effect 
            => ctx.Actor.IsConscious() && (effect.EffectData.EffectType == EffectType.ApplyVulnerable || effect.EffectData.EffectScope.Value == "Vulnerable") && effect.Target.Members.Any(x => x.Id == ctx.Possessor.Id) },
        { ReactionConditionType.OnShieldBroken, ctx => effect => ctx.Actor.IsConscious() && WentToZero(Select(effect, ctx.Possessor, m => m.State[TemporalStatType.Shield])) },
        { ReactionConditionType.OnDamagedHp, ctx => effect => ctx.Actor.IsConscious() && Decreased(Select(effect, ctx.Possessor, m => m.State.Hp))},
        { ReactionConditionType.OnDamaged, ctx => effect => ctx.Actor.IsConscious() && Decreased(Select(effect, ctx.Possessor, m => m.State.Hp + m.State.Shield))},
        { ReactionConditionType.OnBlinded, ctx => effect => ctx.Actor.IsConscious() && Increased(Select(effect, ctx.Possessor, m => m.State[TemporalStatType.Blind])) },
        { ReactionConditionType.OnCausedStun, ctx => effect =>
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return false;
                
                var targetMembers = effect.Target.Members;
                var stunsBefore = targetMembers.Select(t => effect.BattleBefore.Members[t.Id])
                    .Sum(x => x.State[TemporalStatType.CardStun] + x.State[TemporalStatType.Disabled]);
                var stunsAfter = targetMembers.Select(t => effect.BattleAfter.Members[t.Id])
                    .Sum(x => x.State[TemporalStatType.CardStun] + x.State[TemporalStatType.Disabled]);
                return stunsAfter > stunsBefore;
            }
        },
        { ReactionConditionType.OnSlay, ctx => effect =>
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return false;
                
                var targetMembers = effect.Target.Members;
                var unconsciousBefore = targetMembers.Select(t => effect.BattleBefore.Members[t.Id])
                    .Count(x => x.IsUnconscious());
                var unconsciousAfter = targetMembers.Select(t => effect.BattleAfter.Members[t.Id])
                    .Count(x => x.IsUnconscious());
                return unconsciousAfter > unconsciousBefore;
            } },
        { ReactionConditionType.OnCausedHeal, ctx => effect =>
            {
                if (!Equals(ctx.Possessor, effect.Source) || ctx.Actor.IsUnconscious())
                    return false;
                
                var targetMembers = effect.Target.Members;
                var hpBefore = targetMembers.Select(t => effect.BattleBefore.Members[t.Id])
                    .Sum(x => x.State.Hp);
                var hpAfter = targetMembers.Select(t => effect.BattleAfter.Members[t.Id])
                    .Sum(x => x.State.Hp);
                return hpAfter > hpBefore;
            }
        },
        { ReactionConditionType.OnDeath, ctx => effect => ctx.Possessor.IsUnconscious() && (ctx.Possessor.Id == ctx.Actor.Id || ctx.Actor.IsConscious()) },
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
                //TODO: Couldn't find a way to check that ammo was spent on this effect
                return ctx.Possessor.State.PrimaryResourceAmount == 0;
            }
        },
        { ReactionConditionType.OnTagPlayed, ctx => effect 
            => IsRelevant(ReactionConditionType.OnTagPlayed, effect, ctx) 
               && effect.Card.IsPresentAnd(x => x.Type.Tags.Contains(ctx.ReactionEffectScope.EnumVal<CardTag>()))},
        { ReactionConditionType.OnArchetypePlayed, ctx => effect => ctx.Actor.IsConscious() 
           && effect.Card.IsPresentAnd(x => x.Archetypes.Contains(ctx.ReactionEffectScope))},
        { ReactionConditionType.OnDodged, ctx => effect => ctx.Possessor.IsConscious() 
           && effect.Preventions.IsDodging(ctx.Possessor) 
           && Decreased(Select(effect, ctx.Possessor, m => m.State[TemporalStatType.Dodge]))},
        { ReactionConditionType.OnAegised, ctx => effect => ctx.Possessor.IsConscious() 
           && effect.Preventions.IsAegising(ctx.Possessor) 
           && Decreased(Select(effect, ctx.Possessor, m => m.State[TemporalStatType.Aegis]))},
        { ReactionConditionType.OnNearDeath, ctx => effect => ctx.Actor.IsConscious() && ctx.Possessor.CurrentHp() == 1 },
        { ReactionConditionType.OnAllyDeath, ctx => effect => ctx.Actor.IsConscious() 
            && effect.BattleBefore.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious()) 
              > effect.BattleAfter.Members.Count(x => x.Value.TeamType == ctx.Possessor.TeamType && x.Value.IsConscious()) },
        { ReactionConditionType.OnAfflicted, ctx => effect => ctx.Actor.IsConscious() && Increased(Select(effect, ctx.Possessor, m => m.State.StatusesOfType[StatusTag.DamageOverTime])) }
    };

    private static bool IsRelevant(ReactionConditionType type, EffectResolved effect, ReactionConditionContext ctx)
        => (effect.EffectData.EffectType != EffectType.ReactWithCard || effect.EffectData.ReactionConditionType != type)
           && (effect.EffectData.EffectType != EffectType.ReactWithEffect || effect.EffectData.ReactionConditionType != type)
           && ctx.Actor.IsConscious();
    
    private static bool WentToZero(int[] values) => values.First() > 0 && values.Last() == 0;
    private static bool Decreased(int[] values)
    {
        DevLog.Write($"Before: {values.First()}. After: {values.Last()}");
        return values.Last() < values.First();
    }

    private static bool Increased(int[] values) => values.Last() > values.First();
    
    private static int[] Select(EffectResolved e, Member possessor, Func<MemberSnapshot, int> selector)
        => new[] {selector(e.BattleBefore.Members[possessor.Id]), selector(e.BattleAfter.Members[possessor.Id])};
    
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly string _reactionEffectContext;
    private readonly StatusDetail _status;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly Maybe<CardReactionSequence> _reactionEffect;
    private readonly Maybe<ReactionCardType> _reactionCard;
    private readonly ReactionConditionType _conditionType;
    private readonly Func<ReactionConditionContext, Func<EffectResolved, bool>> _conditionBuilder;

    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, string reactionBonusEffectContext,
        ReactiveTriggerScope triggerScope, ReactionConditionType conditionType, ReactionCardType reactionCard)
            : this(isDebuff, numberOfUses, maxDurationTurns, status, reactionBonusEffectContext, triggerScope, conditionType, Maybe<CardReactionSequence>.Missing(), 
                new Maybe<ReactionCardType>(reactionCard, true)) {}
    
    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, string reactionBonusEffectContext,
        ReactiveTriggerScope triggerScope, ReactionConditionType conditionType, CardReactionSequence reactionEffect)
        : this(isDebuff, numberOfUses, maxDurationTurns, status, reactionBonusEffectContext, triggerScope, conditionType, new Maybe<CardReactionSequence>(reactionEffect, true), 
            Maybe<ReactionCardType>.Missing()) {}
        
    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, string reactionBonusEffectContext,
        ReactiveTriggerScope triggerScope, ReactionConditionType conditionType, Maybe<CardReactionSequence> reactionEffect, Maybe<ReactionCardType> reactionCard)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _status = status;
        _triggerScope = triggerScope;
        _reactionEffect = reactionEffect;
        _reactionCard = reactionCard;
        _conditionType = conditionType;
        _conditionBuilder = Conditions.VerboseGetValue(conditionType, conditionType.ToString());
        _reactionEffectContext = reactionBonusEffectContext;
        if (reactionCard.IsMissing && reactionEffect.IsMissing)
            Log.Info($"React With neither has an Effect nor a Card.");
    }
    
    public void Apply(EffectContext ctx)
    {
        if (_reactionCard.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                var reactionConditionContext = new ReactionConditionContext { ReactionEffectScope = _reactionEffectContext };
                reactionConditionContext.Possessor = ctx.BattleMembers.VerboseGetValue(m.MemberId, nameof(ctx.BattleMembers));
                reactionConditionContext.Actor = _reactionCard.Value.ActionSequence.Reactor == ReactiveMember.Originator ? ctx.Source : reactionConditionContext.Possessor;
                m.AddReactiveState(new ReactWithCard(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope,
                    ctx.BattleMembers, m.MemberId, ctx.Source, _reactionCard.Value,
                    _conditionBuilder(reactionConditionContext)));
                DevLog.Write($"Applied React With Card {_conditionType} to {m.Name}");
            });
        else if (_reactionEffect.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                var reactionConditionContext = new ReactionConditionContext { ReactionEffectScope = _reactionEffectContext };
                reactionConditionContext.Possessor = ctx.BattleMembers.VerboseGetValue(m.MemberId, nameof(ctx.BattleMembers));
                reactionConditionContext.Actor = _reactionEffect.Value.Reactor == ReactiveMember.Originator ? ctx.Source : reactionConditionContext.Possessor;
                m.AddReactiveState(new ReactWithEffect(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope,
                    ctx.BattleMembers, m.MemberId, ctx.Source, _reactionEffect.Value,
                    _conditionBuilder(reactionConditionContext)));
                DevLog.Write($"Applied React With Effect {_conditionType} to {m.Name}");
            });
    }
}

public sealed class ReactWithEffect : ReactiveEffectV2Base
{
    public ReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, CardReactionSequence reaction, Func<EffectResolved, bool> condition)
            : base(isDebuff, maxDurationTurns, numberOfUses, status, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
                effect =>
                {
                    var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                    var conditionMet = condition(effect);
                    DevLog.Write($"{status.Tag} Reaction - Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                    return isInTriggerScope && conditionMet;
                })) {}
}

public sealed class ReactWithCard : ReactiveEffectV2Base
{
    public ReactWithCard(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction, Func<EffectResolved, bool> condition)
        : base(isDebuff, maxDurationTurns, numberOfUses, status, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
            effect =>
            {
                var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                var conditionMet = condition(effect);
                DevLog.Write($"{status.Tag} Reaction - Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                return isInTriggerScope && conditionMet;
            })) {}
}

