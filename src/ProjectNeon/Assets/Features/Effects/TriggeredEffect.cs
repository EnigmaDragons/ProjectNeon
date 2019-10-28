using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TriggeredEffect : Effect
{
    private Effect origin;
    private List<GameEvent> triggeredUpon;

    public TriggeredEffect(Effect origin, List<GameEvent> triggeredUpon)
    {
        this.origin = origin;
        this.triggeredUpon = triggeredUpon;
        triggeredUpon.ForEach(evt => evt.Subscribe(new GameEventSubscription(evt.name, x => ProcessEvent(evt), this)));
    }

    void Effect.Apply(Member source, Target target)
    {
        origin.Apply(source, target);
    }
}
