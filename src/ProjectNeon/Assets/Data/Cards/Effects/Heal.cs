using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Heal : Effect
{
    [SerializeField] private int quantity;

    public override void Apply(Target target)
    {
        target.targets.ForEach(member => member.hp += this.quantity);
    }
}
