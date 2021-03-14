using System;
using UnityEngine;

public class SpellDamage : DamageCalculation
{
    private readonly Func<EffectContext, Member, float> _damageCalc;
    private readonly Func<EffectContext, float> _nonMemberDamageCalc;

    public SpellDamage(int baseAmount, float multiplier) : this((ctx, m) => baseAmount + ctx.Source.State.Magic() * multiplier, (ctx) => baseAmount + ctx.Source.State.Magic() * multiplier) {}
    public SpellDamage(Func<EffectContext, Member, float> damageCalc, Func<EffectContext, float> nonMemberDamageCalc)
    {
        _damageCalc = damageCalc;
        _nonMemberDamageCalc = nonMemberDamageCalc;
    }
    
    public SpellDamage WithFactor(float factor) => new SpellDamage((ctx, m) => _damageCalc(ctx, m) * factor, (ctx) => _nonMemberDamageCalc(ctx) * factor);
    public SpellDamage WithAdjustment(float amount) => new SpellDamage((ctx, m) => _damageCalc(ctx, m) + amount, (ctx) => _nonMemberDamageCalc(ctx) + amount);

    public int Calculate(EffectContext ctx, Member target)
    {        
        var amount = Mathf.CeilToInt(_damageCalc(ctx, target) - target.State.Resistance());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 magic damage");
        return amount;
    }

    public int Calculate(EffectContext ctx)
    {
        return Mathf.CeilToInt(_nonMemberDamageCalc(ctx));
    }
}
