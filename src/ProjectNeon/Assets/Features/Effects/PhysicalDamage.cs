using UnityEngine;

public sealed class PhysicalDamage : DamageCalculation
{
    public float Multiplier { get; }

    public PhysicalDamage(float multiplier)
    {
        Multiplier = multiplier;
    }

    public int Calculate(Member source, Member target)
    {
        var amount = Mathf.CeilToInt(source.State.Attack() * Multiplier - target.State.Armor());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 physical damage");
        return amount;
    }
}
