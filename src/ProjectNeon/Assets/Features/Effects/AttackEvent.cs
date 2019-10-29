using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AttackEvent : GameEvent
{
    private Member attacker;
    public Member Attacker { get { return this.attacker; } }
    private Target target;
    public Target Target { get { return this.target; } }

    public AttackEvent(Member attacker, Target target)
    {
        this.attacker = attacker;
        this.target = target;
    }

}
