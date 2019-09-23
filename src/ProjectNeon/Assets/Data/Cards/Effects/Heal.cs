using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Heal : Effect
{
    [SerializeField] private int quantity;

    public override void Apply(Target target)
    {
        if (target.GetType() == typeof(Member))
        {
            ((Member)target).hp += this.quantity;
        }
        else if (target.GetType() == typeof(Team))
        {
            ((Team)target).members.ForEach(
                member => member.hp += this.quantity
            );
        }
    }
}
