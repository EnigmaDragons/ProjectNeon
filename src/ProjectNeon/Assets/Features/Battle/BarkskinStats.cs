
using System;

[Obsolete("Test Effect. Not for real use.")]
public class BarkskinStats : BattleStats
{
    private float quantity = 10F;
    public override float Armor => Stats.Armor + quantity;
}
