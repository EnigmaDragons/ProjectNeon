using UnityEngine;

public sealed class PhysicalDamage : DamageCalculation
{
    private int _baseAmount;
    public float Multiplier { get; }

    public PhysicalDamage(float multiplier)
        : this(0, multiplier) {}
    
    public PhysicalDamage(int baseAmount, float multiplier)
    {
        _baseAmount = baseAmount;
        Multiplier = multiplier;
    }
    
    public PhysicalDamage WithFactor(float factor) => new PhysicalDamage(Multiplier * factor);

    public int Calculate(Member source, Member target)
    {
        var amount = Mathf.CeilToInt(source.State.Attack() * Multiplier + _baseAmount - target.State.Armor());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 physical damage");
        return amount;
    }
}
