public sealed class SpellFlatDamageEffect : Effect
{
    private Member _attacker;
    private Target _target;
    private readonly Effect _effect;

    public SpellFlatDamageEffect(float quantity)
    {
        _effect = new Damage(new SpellFlatDamage(quantity));
    }
    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(m => _effect.Apply(ctx.Source, m));
    }
}
