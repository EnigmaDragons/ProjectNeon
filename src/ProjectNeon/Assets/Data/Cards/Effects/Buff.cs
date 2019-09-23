using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Effect
{
    [SerializeField] private Effect buff;
    [SerializeField] private int turns;
    private int count = 0;

    public override void Apply(Target target)
    {
        if (count < turns)
        {
            buff.Apply(target);
            count++;
        }
    }
}
