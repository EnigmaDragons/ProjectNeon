using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class TriggeredEffect<T> : Effect where T : GameEvent
{
    private Effect origin;
    private T trigger;

    public void Init(Effect origin, T trigger) 
    {
        Debug.Log("Initialçizing triggered effect");
        Debug.Log("Event name: " + trigger.name);
        Debug.Log("Event: " + trigger.GetHashCode());
        trigger.Subscribe(new GameEventSubscription(trigger.name, x=> ProcessEvent(trigger), this));
    }

    public abstract void Apply(Member source, Target target);
    public abstract void ProcessEvent(T evt);
}
