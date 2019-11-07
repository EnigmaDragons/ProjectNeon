using System;

public sealed class PhysicalDamage : Damage
{
    public float Multiplier { get; }

    public PhysicalDamage(float multiplier)
    {
        Multiplier = multiplier;
    }

    public int Calculate(Member source, Target target)
    {
        return Convert.ToInt32(Math.Ceiling(source.State.Attack() * Multiplier * ((1f - target.Members[0].State.Armor()) / 1f)));    
    }

}
