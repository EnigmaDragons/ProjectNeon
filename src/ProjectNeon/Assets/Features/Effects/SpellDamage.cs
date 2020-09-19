using UnityEngine;

public class SpellDamage : DamageCalculation
{
    private readonly float _multiplier;

    public SpellDamage(float multiplier)
    {
        _multiplier = multiplier;
    }
    
    public SpellDamage WithFactor(float factor) => new SpellDamage(_multiplier * factor);

    public int Calculate(Member attacker, Member target)
    {        
        var amount = Mathf.CeilToInt(attacker.State.Magic() * _multiplier - target.State.Resistance());
        if (amount < 1)
            Log.Warn($"{target.Name} is taking 0 magic damage");
        return amount;
    }
}
