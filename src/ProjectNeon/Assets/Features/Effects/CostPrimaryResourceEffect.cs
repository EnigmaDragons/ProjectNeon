using System;
using UnityEngine;

public sealed class CostPrimaryResourceEffect : Effect
{
    private Effect _effect;
    private int _cost;

    public CostPrimaryResourceEffect(Effect effect, int cost)
    {
        _effect = effect;
        _cost = cost;
    }

    void Effect.Apply(Member source, Target target)
    {
        _effect.Apply(source, target);
        source.State.SpendPrimaryResource(_cost);
    }
}