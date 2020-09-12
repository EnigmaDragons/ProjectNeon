using System;
using System.Collections.Generic;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> CreateEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.HealFlat, e => new Heal(e.IntAmount) },
        { EffectType.PhysicalDamage, e => new DealDamage(new PhysicalDamage(e.FloatAmount)) },
        { EffectType.AdjustStatAdditively, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(
            new AdjustedStats(new StatAddends().WithRaw(e.EffectScope, e.IntAmount), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns == -1, StatusTag.None)))},
        { EffectType.AdjustStatMultiplicatively, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(
            new AdjustedStats(new StatMultipliers().WithRaw(e.EffectScope, e.FloatAmount), e.NumberOfTurns, e.IntAmount < 0, e.IntAmount == -1, StatusTag.None)))},
        { EffectType.AdjustTemporaryStatAdditively, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(
            new AdjustedStats(new StatAddends().WithRaw(e.EffectScope, e.IntAmount), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns == -1, StatusTag.None)))},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => m.RemoveTemporaryEffects(effect => effect.IsDebuff))},
        { EffectType.ShieldFlat, e => new ShieldFlat(e.IntAmount) },
        { EffectType.ResourceFlat, e => new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.IntAmount))},
        { EffectType.DamageOverTimeFlat, e => new DamageOverTime(e) },
        { EffectType.ApplyVulnerable, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(
            new AdjustedStats(new StatMultipliers().With(StatType.Damagability, 1.33f), e.NumberOfTurns, true, e.NumberOfTurns == -1, StatusTag.Vulnerable))) },
        { EffectType.ShieldToughness, e => new SimpleEffect((src, m) => m.GainShield(e.IntAmount * src.State.Toughness())) },
        { EffectType.RemoveShields, e => new SimpleEffect((src, m) => m.GainShield(-999)) },
        { EffectType.StunForTurns, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new StunForTurns(e.NumberOfTurns)))},
        { EffectType.StunForNumberOfCards, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(AdjustedStats.CreateIndefinite(new StatAddends().With(TemporalStatType.CardStun, e.IntAmount), true))) },
        { EffectType.StealLifeNextAttack, e => new NoEffect() }, // TODO: Implement Life Steal
        { EffectType.InterceptAttackForTurns, e => new InterceptAttack(e.NumberOfTurns)},
        { EffectType.Attack, e => new Attack(e.FloatAmount, e.HitsRandomTargetMember)},
        { EffectType.EvadeAttacks, e => new Evade(e.IntAmount) },
        { EffectType.HealOverTime, e => new HealOverTime(e.FloatAmount, e.NumberOfTurns) },
        { EffectType.ExcludeSelfFromEffect, e => new ExcludeSelfFromEffect(Create(e.origin)) },
        { EffectType.ShieldBasedOnShieldValue, e => new SimpleEffect((src, m) => m.GainShield(e.FloatAmount * src.State[TemporalStatType.Shield])) },
        { EffectType.OnAttacked, e => new EffectOnAttacked(false, e.IntAmount, e.NumberOfTurns, e.StatusTag, ReactiveTriggerScopeExtensions.Parse(e.EffectScope), e.ReactionSequence) },
        { EffectType.OnEvaded, e => new EffectOnEvaded(false, e.IntAmount, e.NumberOfTurns, ReactiveTriggerScopeExtensions.Parse(e.EffectScope),e.ReactionSequence) },
        { EffectType.OnShieldBroken, e => new EffectOnShieldBroken(false, e.NumberOfTurns, ReactiveTriggerScopeExtensions.Parse(e.EffectScope),e.ReactionSequence) },
        { EffectType.OnDamaged, e => new EffectOnDamaged(false, e.NumberOfTurns, e.IntAmount, e.ReactionSequence) },
        { EffectType.CostResource, e => new SimpleEffect(m => m.Lose(new ResourceQuantity { Amount = e.IntAmount, ResourceType = e.EffectScope.Value }))},
        { EffectType.AnyTargetHealthBelowThreshold, e => new AnyTargetHealthBelowThreshold(Create(e.origin), e.FloatAmount) },
        { EffectType.SpellFlatDamageEffect, e => new SpellFlatDamageEffect(e.IntAmount) },
        { EffectType.ApplyOnShieldBelowValue, e => new ApplyOnShieldBelowValue(Create(e.origin), e.IntAmount) },
        { EffectType.ApplyOnChance, e => new ApplyOnChance(Create(e.origin), e.FloatAmount) },
        { EffectType.HealPrimaryResource, e => new SimpleEffect((src, m) => m.GainHp(src.State.PrimaryResourceAmount)) },
        { EffectType.HealMagic, e => new HealMagic(e.FloatAmount) },
        { EffectType.AdjustPrimaryResource, e => new SimpleEffect(m => m.AdjustPrimaryResource(e.IntAmount)) },
        { EffectType.AdjustPlayerStats, e => new PlayerEffect(p => p.AddState(
            new AdjustedPlayerStats(new PlayerStatAddends().With((PlayerStatType)Enum.Parse(typeof(PlayerStatType), e.EffectScope), e.IntAmount), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns < 0))) },
        { EffectType.AdjustStatAdditivelyBaseOnMagicStat, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(
            new AdjustedStats(new StatAddends().WithRaw(e.EffectScope, Mathf.CeilToInt(e.IntAmount * m[StatType.Magic])), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns == -1, StatusTag.None)))},
        { EffectType.DamageSpell, e => new MagicAttack(e.FloatAmount, e.HitsRandomTargetMember) },
        { EffectType.ApplyTaunt, e => new SimpleEffect(m => m.Adjust(TemporalStatType.Taunt, e.NumberOfTurns))}
    };
    
    public static void Apply(EffectData effectData, EffectContext ctx)
    {
        var effect = Create(effectData);
        BattleLog.Write($"Applying Effect of {effectData.EffectType} to {ctx.Target.MembersDescriptions()}");
        effect.Apply(ctx);
    }

    public static Effect Create(EffectData effectData)
    {
        try
        {
            var effectType = effectData.EffectType;
            if (!CreateEffectOfType.ContainsKey(effectData.EffectType))
            {
                Log.Error($"No EffectType of {effectData.EffectType} exists in {nameof(AllEffects)}");
                return CreateEffectOfType[EffectType.Nothing](effectData);
            }

            return CreateEffectOfType[effectType](effectData);
        }
        catch (Exception e)
        {
            Log.Error($"EffectType {effectData.EffectType} is broken {e}");
            return CreateEffectOfType[EffectType.Nothing](effectData);
        }
    }
}
