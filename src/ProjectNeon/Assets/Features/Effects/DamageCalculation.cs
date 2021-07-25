public interface DamageCalculation 
{
    int Calculate(EffectContext ctx, Member member);

    DamageCalculation WithFactor(float factor);
}

