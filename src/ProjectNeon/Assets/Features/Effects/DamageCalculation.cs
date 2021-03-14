public interface DamageCalculation 
{
    int Calculate(EffectContext ctx, Member member);
    int Calculate(EffectContext ctx);
}