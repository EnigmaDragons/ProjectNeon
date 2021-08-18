using System;
using UnityEngine;

public sealed class PhysicalDamage : DamageCalculation
{
    private readonly Func<EffectContext, Member, float> _damageCalc;

    public PhysicalDamage(Func<EffectContext, Member, float> damageCalc) => _damageCalc = damageCalc;

    public bool DealRawDamage => false;
    
    public DamageCalculation WithFactor(float factor) => new PhysicalDamage((ctx, m) => Mathf.CeilToInt(_damageCalc(ctx, m)) * factor);

    public int Calculate(EffectContext ctx, Member target)
    {
        var amount = Mathf.CeilToInt(_damageCalc(ctx, target) - target.State.Armor());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 physical damage");
        return amount;
    }
}
