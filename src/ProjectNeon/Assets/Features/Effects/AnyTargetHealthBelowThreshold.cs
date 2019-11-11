using System;
using UnityEngine;

public sealed class AnyTargetHealthBelowThreshold : ConditionalEffect
{
    private float _threshold;

    public AnyTargetHealthBelowThreshold (Effect effect, float threshold) : base(effect)
    {
        _threshold = threshold;
    }

    public override bool Condition()
    {
        return Array.Exists(_target.Members, element => element.State[TemporalStatType.HP] < element.State.MaxHP() * _threshold );
    }
}