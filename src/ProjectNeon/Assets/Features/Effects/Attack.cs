
public sealed class Attack : Effect
{
    private readonly PhysicalDamage _damage;
    private readonly bool _hitsRandomTarget;
    
    public Attack(PhysicalDamage damage, bool hitsRandomTarget = false)
    {
        _damage = damage;
        _hitsRandomTarget = hitsRandomTarget;
    }

    public void Apply(EffectContext ctx)
    {
        AttackProcessing.Apply(_damage, _hitsRandomTarget, ctx);
        Message.Publish(new Finished<Attack> { Message = this });
    }
}
