using System.Linq;
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
        return Target.Members.Any(e => e.State[TemporalStatType.HP] < e.State.MaxHp() * _threshold );
    }
}