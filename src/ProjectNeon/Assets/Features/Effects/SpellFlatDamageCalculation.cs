using UnityEngine;

public sealed class SpellFlatDamage : DamageCalculation
{
    private float Quantity { get; }

    public SpellFlatDamage(float quantity)
    {
        Quantity = quantity;
    }

    public int Calculate(Member source, Member target)
    {
        return Mathf.CeilToInt(Quantity - target.State.Resistance());
    }
}
