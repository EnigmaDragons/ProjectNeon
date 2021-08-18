public interface DamageCalculation 
{
    bool DealRawDamage { get; }
    
    int Calculate(EffectContext ctx, Member member);

    DamageCalculation WithFactor(float factor);
}

