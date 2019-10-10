using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkskinStats : BattleStats
{
    private float quantity = 10F;

    public new float Armor()
    {
        return this.Origin.Armor + quantity;
    }
}
