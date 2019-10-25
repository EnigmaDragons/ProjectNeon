using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TriggeredEffect : Effect
{
    private Effect origin;
    private List<GameEvent> triggeredUpon;
    private Dictionary<GameEvent, Boolean> trigger;

    public TriggeredEffect(Effect origin, List<GameEvent> triggeredUpon)
    {
        this.origin = origin;
        this.triggeredUpon = triggeredUpon;
        trigger = this.triggeredUpon.ToDictionary(key => key, value => false);
        );
    }

    void Effect.Apply(Member source, Target target)
    {
        if (!trigger.ContainsValue(false))
        {
            origin.Apply(source, target);
        }
        
    }
}
