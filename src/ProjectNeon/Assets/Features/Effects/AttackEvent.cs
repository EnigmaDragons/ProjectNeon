using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class AttackEvent : GameEvent
{

    private Member attacker;
    public Member Attacker { get { return this.attacker; } }
    private Target target;
    public Target Target { get { return this.target; } }

    public void Init(Member attacker, Target target)
    {
        this.attacker = attacker;
        this.target = target;
    }
}
