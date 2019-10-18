using System;
using System.Collections.Generic;
using Features.Combatants;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> _createEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.HealFlat, e => new SimpleEffect(m => m.GainHp(e.IntAmount))},
        { EffectType.PhysicalDamage, e => new SimpleEffect((src, m) => m.TakePhysicalDamage(src.State.Attack * e.FloatAmount))},
        { EffectType.BuffAttackFlat, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new BuffedStats(new StatAddends{ Attack = e.IntAmount}, e.NumberOfTurns)))},
        { EffectType.BuffAttackMultiplier, e => new SimpleEffect(m => m.ApplyTemporatyMultiplier(new BuffedStats(new StatMultipliers{ Attack = e.FloatAmount}, e.NumberOfTurns)))},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => m.RemoveTemporaryEffects(effect => effect.IsDebuff))},
        { EffectType.BuffMaxHp, e => new SimpleEffect(m =>
            {
                m.ApplyAdditiveUntilEndOfBattle(new StatAddends {MaxHP = e.IntAmount});
                m.GainHp(e.IntAmount);
            })},
        { EffectType.ShieldFlat, e => new SimpleEffect(m => m.GainShield(e.IntAmount)) },
        { EffectType.ResourceFlat, e => new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.IntAmount))},
        { EffectType.DamageOverTimeFlat, e => new DamageOverTime(e) },
    };
    
    public static void Apply(EffectData effectData, Member source, Target target)
    {
        Create(effectData).Apply(source, target);
    }

    public static Effect Create(EffectData effectData)
    {
        var effectType = effectData.EffectType;
        if (!_createEffectOfType.ContainsKey(effectData.EffectType))
        {
            Debug.LogError($"No EffectType of {effectData.EffectType} exists in {nameof(AllEffects)}");
            return _createEffectOfType[EffectType.Nothing](effectData);
        }
        return _createEffectOfType[effectType](effectData);
    }
}
