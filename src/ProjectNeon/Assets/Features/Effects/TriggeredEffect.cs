using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TriggeredEffect<T> : Effect where T : GameEvent
{
    [SerializeField] protected Effect origin;

    [SerializeField] protected List<T> triggeredUpon;

    public void Init()
    {
        triggeredUpon.ForEach(evt => evt.Subscribe(new GameEventSubscription(evt.name, x => ProcessEvent(evt), this)));
    }

    public void Apply(Member source, Target target)
    {
        origin.Apply(source, target);
    }

    public abstract void ProcessEvent(T evt);
}
