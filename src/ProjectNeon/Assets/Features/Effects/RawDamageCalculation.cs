using System;

public class RawDamageCalculation : DamageCalculation
{
    private readonly Func<EffectContext, Member, float> _damageCalc;

    public RawDamageCalculation(Func<EffectContext, Member, float> damageCalc) => _damageCalc = damageCalc;

    public bool DealRawDamage => true;

    public int Calculate(EffectContext ctx, Member member) => _damageCalc(ctx, member).CeilingInt();

    public DamageCalculation WithFactor(float factor) => new RawDamageCalculation((ctx, m) => _damageCalc(ctx, m) * factor);
}