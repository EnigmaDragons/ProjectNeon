using System;

public class TrueDamageCalculation : DamageCalculation
{
    private readonly Func<EffectContext, Member, float> _damageCalc;

    public TrueDamageCalculation(Func<EffectContext, Member, float> damageCalc) => _damageCalc = damageCalc;

    public bool DealTrueDamage => true;

    public int Calculate(EffectContext ctx, Member member) => _damageCalc(ctx, member).CeilingInt();

    public DamageCalculation WithFactor(float factor) => new TrueDamageCalculation((ctx, m) => _damageCalc(ctx, m) * factor);
}