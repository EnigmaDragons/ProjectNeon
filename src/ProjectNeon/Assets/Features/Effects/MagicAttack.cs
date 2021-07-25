public class MagicAttack : Effect
{
    private readonly SpellDamage _damageCalc;
    private readonly bool _hitsRandomTarget;

    public MagicAttack(SpellDamage damageCalc, bool hitsRandomTarget)
    {
        _damageCalc = damageCalc;
        _hitsRandomTarget = hitsRandomTarget;
    }
    
    public void Apply(EffectContext ctx)
    {
        AttackProcessing.Apply(_damageCalc, _hitsRandomTarget, ctx);
        Message.Publish(new Finished<MagicAttack> { Message = this });
    }
}
