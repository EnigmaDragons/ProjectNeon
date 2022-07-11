public interface DamageCalculation 
{
    bool DealTrueDamage { get; }
    
    int Calculate(EffectContext ctx, Member member);

    DamageCalculation WithFactor(float factor);
}

