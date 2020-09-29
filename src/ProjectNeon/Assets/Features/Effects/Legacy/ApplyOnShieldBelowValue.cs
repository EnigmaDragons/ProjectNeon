using System.Linq;
using UnityEngine;

public sealed class ApplyOnShieldBelowValue : ConditionalEffect
{
    private int _value;

    public ApplyOnShieldBelowValue(Effect effect, int value) : base(effect)
    {
        _value = value;
    }

    public override bool Condition()
    {
        return Target.Members.Any(e => e.State[TemporalStatType.Shield] < _value );
    }
}