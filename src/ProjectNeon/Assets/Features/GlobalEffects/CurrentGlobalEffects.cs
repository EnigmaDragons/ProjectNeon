using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="GlobalEffects/CurrentGlobalEffects")]
public class CurrentGlobalEffects : ScriptableObject
{
    [SerializeField] private List<GlobalEffect> globalEffects = new List<GlobalEffect>();

    private float _cardShopPriceFactor = 1f;
    
    public GlobalEffect[] Value => globalEffects.ToArray();

    public void Clear() => PublishAfter(() => 
        globalEffects.Clear());

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

    public void SetCardPriceFactor(float factor) => PublishAfter(() => _cardShopPriceFactor = factor);
    public void AdjustCardPriceFactor(Func<float, float> getNewFactor) => PublishAfter(() => _cardShopPriceFactor = getNewFactor(CardShopPriceFactor));
    public float CardShopPriceFactor => _cardShopPriceFactor;
}