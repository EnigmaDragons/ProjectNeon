using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttackEffect : TriggeredEffect<AttackEvent>
{
    private Effect origin;

    public OnAttackEffect(Effect origin)
    {
        this.origin = origin;
    }

    public override void Apply(Member source, Target target)
    {
        Debug.Log("Applying event on card play");
        AttackEvent evt = (AttackEvent)ScriptableObject.CreateInstance("AttackEvent");
        evt.Init(source, target);
        evt.name = "AttackEvent";
        this.Init(origin, evt);
    }

    public override void ProcessEvent(AttackEvent evt)
    {
        Debug.Log("Applying event on event raise");
        this.origin.Apply(evt.Attacker, evt.Target);
    }
}
