using System;
using System.Linq;
using UnityEngine;

public sealed class PhysicalDamage : DamageCalculation
{
    public float Multiplier { get; }

    public PhysicalDamage(float multiplier)
    {
        Multiplier = multiplier;
    }

    public int Calculate(Member source, Target target)
    {
        var amount = Mathf.CeilToInt(source.State.Attack() * Multiplier * ((1f - (target.Members[0].State.Armor() / 100f)) / 1f));
        if (amount < 1)
            Debug.Log($"{target.Members.First().Name} is taking 0 physical damage");
        return amount;
    }
}
