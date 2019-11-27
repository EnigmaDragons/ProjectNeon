using System;
using UnityEngine;

public sealed class SpellFlatDamage : DamageCalculation
{
    public float Quantity { get; }

    public SpellFlatDamage(float quantity)
    {
        Quantity = quantity;
    }

    public int Calculate(Member source, Target target)
    {
        return Mathf.CeilToInt(Quantity * ((1f - target.Members[0].State.Resistance()) / 1f));
    }

}
