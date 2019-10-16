using System;
using System.Collections.Generic;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> _createEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.HealFlat, e => new SimpleEffect(t => t.ApplyToAll(m => m.GainHp(e.FloatAmount)))},
        { EffectType.PhysicalDamage, e => new SimpleEffect((src, t) => t.ApplyToAll(m => m.TakePhysicalDamage(src.State.Attack * e.FloatAmount)))}
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
