using UnityEngine;
using System.Collections;

/**
 * Apply an effect while there is enough Primary Resource
 */
public class RepeatOnPrimaryResource : Effect
{
    private Effect _effect;

    public RepeatOnPrimaryResource(Effect effect)
    {
        _effect = effect;
    }

    void Effect.Apply(Member source, Target target)
    {
        while (source.State[source.State.ResourceTypes[0]]  > 0)
        {
            _effect.Apply(source, target);
        }
    }
}
