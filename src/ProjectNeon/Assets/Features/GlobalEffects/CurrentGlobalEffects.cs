using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="GlobalEffects/CurrentGlobalEffects")]
public class CurrentGlobalEffects : ScriptableObject
{
    [SerializeField] private List<GlobalEffect> globalEffects = new List<GlobalEffect>();
    [SerializeField] private float cardShopPriceFactor = 1f;
    [SerializeField] private float encounterDifficultyFactor = 1f;
    [SerializeField] private List<TargetedEffectData> startOfBattleEffects = new List<TargetedEffectData>();
    
    public GlobalEffect[] Value => globalEffects.ToArray();
    public TargetedEffectData[] StartOfBattleEffects => startOfBattleEffects.ToArray();

    public void SetCardPriceFactor(float factor) => PublishAfter(() => cardShopPriceFactor = factor);
    public void AdjustCardPriceFactor(Func<float, float> getNewFactor) => PublishAfter(() => cardShopPriceFactor = getNewFactor(CardShopPriceFactor));
    public float CardShopPriceFactor => cardShopPriceFactor;

    public void AdjustEncounterDifficultyFactor(Func<float, float> getNewFactor) => PublishAfter(() => encounterDifficultyFactor = getNewFactor(EncounterDifficultyFactor));
    public float EncounterDifficultyFactor => encounterDifficultyFactor;

    public void AddStartOfBattleEffect(TargetedEffectData e) => PublishAfter(() => startOfBattleEffects.Add(e));
    public void RemoveStartOfBattleEffect(TargetedEffectData e) => PublishAfter(() => startOfBattleEffects.Remove(e));
    
    public void Clear() => PublishAfter(() =>
    {
        cardShopPriceFactor = 1f;
        encounterDifficultyFactor = 1f;
        startOfBattleEffects.Clear();
        globalEffects.Clear();
    });

    public void Apply(GlobalEffect e, GlobalEffectContext ctx)
    {
        e.Apply(ctx);
        Add(e);
    }
    
    public void Add(GlobalEffect e) => PublishAfter(() => 
        globalEffects.Add(e));

    public void Remove(GlobalEffect e) => PublishAfter(() => 
        globalEffects.Remove(e));

    private void PublishAfter(Action a)
    {
        a();
        Message.Publish(new GlobalEffectsChanged(Value));
    }
}
