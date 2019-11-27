
using UnityEngine;

public sealed class SpellFlatDamageEffect : Effect
{
    private Member _attacker;
    private Target _target;
    private Effect _effect;
    private DamageCalculation _damage;
    private float _quantity;

    public SpellFlatDamageEffect(float quantity)
    {
        _quantity = quantity;
        _damage = new FlatDamage(_quantity);
        _effect = new Damage(_damage);
    }

    public void Apply(Member source, Target target)
    {
        _attacker = source;
        _target = target;
        if (target.Members.Length > 1)
        {
            target.Members.ForEach(
                member => {
                    new SpellFlatDamageEffect(_quantity).Apply(source, target);
                }
            );
        }
        else
        {
            _effect.Apply(source, target);
        }
    }

    public void Apply(Member source, Member target)
    {
        Apply(source, new Single(target));
    }
}
