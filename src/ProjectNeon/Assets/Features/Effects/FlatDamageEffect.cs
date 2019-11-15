
using UnityEngine;

public sealed class FlatDamageEffect : Effect
{
    public Member Attacker { get; set; }
    public Target Target { get; set; }
    public Effect Effect { get; set; }
    public DamageCalculation Damage { get; set; }
    public float Quantity { get; set; }

    public FlatDamageEffect(float quantity)
    {
        Quantity = quantity;
        Damage = new FlatDamage(Quantity);
        Effect = new Damage(Damage);
    }

    public void Apply(Member source, Target target)
    {
        Attacker = source;
        Target = target;
        if (target.Members.Length > 1)
        {
            target.Members.ForEach(
                member => {
                    new FlatDamageEffect(Quantity).Apply(source, target);
                }
            );
        }
        else
        {
            Effect.Apply(source, target);
        }
    }

    public void Apply(Member source, Member target)
    {
        Apply(source, new Single(target));
    }
}
