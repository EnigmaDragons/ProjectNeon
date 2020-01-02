public sealed class SpellFlatDamageEffect : Effect
{
    private Member _attacker;
    private Target _target;
    private readonly Effect _effect;
    private readonly float _quantity;

    public SpellFlatDamageEffect(float quantity)
    {
        _quantity = quantity;
        _effect = new Damage(new SpellFlatDamage(_quantity));
    }

    public void Apply(Member source, Target target)
    {
        if (target.Members.Length > 1)
            target.Members.ForEach(member => new SpellFlatDamageEffect(_quantity).Apply(source, new Single(member)));
        else
            _effect.Apply(source, target);
    }

    public void Apply(Member source, Member target)
    {
        Apply(source, new Single(target));
    }
}
