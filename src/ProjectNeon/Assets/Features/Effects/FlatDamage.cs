using System;
using UnityEngine;

public sealed class FlatDamage : DamageCalculation
{
    public float Quantity { get; }

    public FlatDamage(float quantity)
    {
        Quantity = quantity;
    }

    public int Calculate(Member source, Target target)
    {
        return Mathf.CeilToInt(Quantity);
    }

}
