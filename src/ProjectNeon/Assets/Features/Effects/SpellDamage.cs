using System;
using UnityEngine;

public class SpellDamage : DamageCalculation
{
    private readonly Func<EffectContext, Member, float> _damageCalc;

    public SpellDamage(Func<EffectContext, Member, float> damageCalc) => _damageCalc = damageCalc;

    public DamageCalculation WithFactor(float factor) => new SpellDamage((ctx, m) => _damageCalc(ctx, m) * factor);

    public bool DealRawDamage => false;

    public int Calculate(EffectContext ctx, Member target)
    {        
        var amount = Mathf.CeilToInt(_damageCalc(ctx, target) - target.State.Resistance());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 magic damage");
        return amount;
    }
}
