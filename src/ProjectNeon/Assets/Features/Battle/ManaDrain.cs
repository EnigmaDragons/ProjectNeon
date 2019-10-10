using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDrain : TurnStats
{
    private int quantity = 10;

    public new int Magic()
    {
        return this.Origin.Magic - quantity;
    }
}
