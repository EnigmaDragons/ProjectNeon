using UnityEngine;
using System.Collections;

/**
 * Effects that trigger on Turn Start.
 */
public class EffectOnTurnStart : Effect
{
    private Effect _effect;

    public EffectOnTurnStart(Effect origin)
    {
        _effect = origin;
    }

    public void Apply(Member source, Target target)
    {
        Message.Subscribe<TurnStart>(_ => Execute(source, target), this);
    }

    void Execute(Member source, Target target)
    {
        _effect.Apply(source, target);
        Message.Unsubscribe(this);        
    }
}
