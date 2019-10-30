using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Attack 
{
    private Member attacker;
    public Member Attacker { get { return this.attacker; } }
    private Target target;
    public Target Target { get { return this.target; } }

    public Attack(Member attacker, Target target)
    {
        this.attacker = attacker;
        this.target = target;
    }
}
