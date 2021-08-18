
public class RawDamageAttack : Effect
{
    private readonly RawDamageCalculation _damage;
    private readonly bool _hitsRandomTarget;
    
    public RawDamageAttack(RawDamageCalculation damage, bool hitsRandomTarget = false)
    {
        _damage = damage;
        _hitsRandomTarget = hitsRandomTarget;
    }

    public void Apply(EffectContext ctx) 
        => AttackProcessing.Apply(_damage, _hitsRandomTarget, ctx);
}