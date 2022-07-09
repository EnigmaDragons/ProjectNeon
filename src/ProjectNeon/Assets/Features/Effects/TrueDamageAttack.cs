
public class TrueDamageAttack : Effect
{
    private readonly TrueDamageCalculation _damage;
    private readonly bool _hitsRandomTarget;
    
    public TrueDamageAttack(TrueDamageCalculation damage, bool hitsRandomTarget = false)
    {
        _damage = damage;
        _hitsRandomTarget = hitsRandomTarget;
    }

    public void Apply(EffectContext ctx) 
        => AttackProcessing.Apply(_damage, _hitsRandomTarget, ctx);
}