﻿using System.Linq;
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
        var amount = Mathf.CeilToInt(source.State.Attack() * Multiplier - target.Members[0].State.Armor());
        if (amount < 1)
            BattleLog.Write($"{target.Members.First().Name} is taking 0 physical damage");
        return amount;
    }
}
