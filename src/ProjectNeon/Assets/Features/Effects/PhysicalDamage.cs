using System;
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
        return Mathf.CeilToInt(source.State.Attack() * Multiplier * ((1f - target.Members[0].State.Armor()) / 1f));
    }

}
