using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Attack 
{
    public Member Attacker { get; }
    public Member Target { get; }

    public Attack(Member attacker, Member target)
    {
        this.Attacker = attacker;
        this.Target = target;
    }
}
