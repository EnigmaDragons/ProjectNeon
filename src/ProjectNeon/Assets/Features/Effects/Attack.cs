using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Attack 
{
    public Member Attacker { get; }
    public Target Target { get; }

    public Attack(Member attacker, Target target)
    {
        this.Attacker = attacker;
        this.Target = target;
    }
}
