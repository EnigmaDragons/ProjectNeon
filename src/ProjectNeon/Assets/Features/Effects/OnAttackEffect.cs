using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttackEffect : TriggeredEffect<AttackEvent>
{

    public override void ProcessEvent(AttackEvent evt)
    {
        this.Apply(evt.Attacker, evt.Target);
    }
}
