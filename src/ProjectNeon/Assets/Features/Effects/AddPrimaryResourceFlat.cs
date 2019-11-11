using System;
using UnityEngine;

public sealed class AddPrimaryResourceFlat : Effect
{
    private int _quantity;

    public AddPrimaryResourceFlat(int quantity)
    {
        _quantity = quantity;
    }

    public void Apply(Member source, Target target)
    {
        source.State.GainPrimaryResource(_quantity);
    }
}