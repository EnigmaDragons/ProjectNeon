using UnityEngine;
using System.Collections;

/**
 * Apply an effect multiple times
 */
public class RepeatEffect : Effect
{
    private Effect _effect;
    private int _repetitions;

    public RepeatEffect(Effect effect, int repetitions)
    {
        _effect = effect;
        _repetitions = repetitions;
    }

    void Effect.Apply(Member source, Target target)
    {
        for (int i = 0; i < _repetitions; i++)
        {
            _effect.Apply(source, target);
        }
    }
}
