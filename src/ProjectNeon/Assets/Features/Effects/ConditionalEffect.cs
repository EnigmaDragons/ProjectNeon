
using System;
using UnityEngine;

public abstract class ConditionalEffect : Effect
{
    protected Effect _effect;
    protected Member _source;
    protected Target _target;


    public ConditionalEffect(Effect effect)
    {
        _effect = effect;
    }

    public void Apply(Member source, Target target)
    {
        _source = source;
        _target = target;
        if (Condition())
        {
            _effect.Apply(source, target);
        }

    }

    public abstract bool Condition();
}