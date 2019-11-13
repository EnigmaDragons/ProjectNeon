
using System;
using UnityEngine;

public abstract class ConditionalEffect : Effect
{
    protected Effect Effect;
    protected Member Source;
    protected Target Target;


    public ConditionalEffect(Effect effect)
    {
        Effect = effect;
    }

    public void Apply(Member source, Target target)
    {
        Source = source;
        Target = target;
        if (Condition())
        {
            Effect.Apply(source, target);
        }

    }

    public abstract bool Condition();
}