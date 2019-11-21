using UnityEngine;
using System.Collections;

/**
 * Apply an effect while there is enough Primary Resource
 */
public class RepeatUntilPrimaryResourceDepleted : Effect
{
    private Effect _effect;
    private int _effectCost;

    public RepeatUntilPrimaryResourceDepleted(Effect effect, int cost)
    {
        _effect = effect;
        _effectCost = cost;
    }

    void Effect.Apply(Member source, Target target)
    {
        while (source.State.PrimaryResourceAmount  >= _effectCost)
        {
            _effect.Apply(source, target);
        }
    }
}
