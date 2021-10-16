using System;
using System.Collections.Generic;

public static class AllGlobalEffects
{
    private static readonly GlobalEffect NoEffect = new FullGlobalEffect(
        new GlobalEffectData {ShortDescription = "None", FullDescription = "None", EffectType = GlobalEffectType.None}, 
        _ => {},
        _ => {});
    
    private static readonly Dictionary<GlobalEffectType, Func<GlobalEffectData, GlobalEffect>> CreateEffectOfType = new Dictionary<GlobalEffectType, Func<GlobalEffectData, GlobalEffect>>
    {
        { GlobalEffectType.None, d => NoEffect },
        { GlobalEffectType.AdjustCardShopPrices, d => new BasicGlobalEffect(d, 
            fx => fx.AdjustCardPriceFactor(f => f * d.FloatAmount), 
            fx => fx.AdjustCardPriceFactor(f => f / d.FloatAmount)) },
        { GlobalEffectType.AdjustEncounterPowerLevel, d => new BasicGlobalEffect(d, 
            fx => fx.AdjustEncounterDifficultyFactor(f => f * d.FloatAmount),
            fx => fx.AdjustEncounterDifficultyFactor(f => f / d.FloatAmount)) },
        { GlobalEffectType.AddStartOfBattleEffect, d => new BasicGlobalEffect(d, 
            fx => fx.AddStartOfBattleEffect(d.BattleEffect),
            fx => fx.RemoveStartOfBattleEffect(d.BattleEffect))},
        { GlobalEffectType.PreventTravelToCorpNodeType, d => new BasicGlobalEffect(d,
            fx => Enum.TryParse(d.EffectScope, out MapNodeType nodeType).If(() => fx.PreventTravelTo(d.CorpName, nodeType)),
            fx => Enum.TryParse(d.EffectScope, out MapNodeType nodeType).If(() => fx.AllowTravelTo(d.CorpName, nodeType))) },
    };

    public static GlobalEffect Create(GlobalEffectData data)
    {
        if (!CreateEffectOfType.ContainsKey(data.EffectType))
        {
            Log.Error($"No EffectType of {data.EffectType} exists in {nameof(AllGlobalEffects)}");
            #if UNITY_EDITOR
            throw new NotImplementedException($"No EffectType of {data.EffectType} exists in {nameof(AllGlobalEffects)}");
            #endif
            return CreateEffectOfType[GlobalEffectType.None](data);
        }

        return CreateEffectOfType[data.EffectType](data);
    }

    public static void Apply(GlobalEffectData data, GlobalEffectContext ctx)
    {
        var e = Create(data);
        e.Apply(ctx);
    }
}
