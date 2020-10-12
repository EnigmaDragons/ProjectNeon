using System;
using UnityEngine;

public sealed class DamageOverTime : Effect
{
    private readonly int _amount;
    private readonly int _turns;
    private readonly float _statMultiplier;
    private readonly StatType _stat;
    private readonly bool _statSelected;

    public DamageOverTime(EffectData data)
    {
        _amount = data.IntAmount;
        _turns = data.NumberOfTurns;
        _statMultiplier = data.FloatAmount;
        _statSelected = Enum.TryParse("StatType", out _stat);
    }

    public void Apply(EffectContext ctx)
    {
        var calculatedAmount = _statSelected ? Mathf.CeilToInt(_statMultiplier * ctx.Source.State[_stat]) : _amount;
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(new DamageOverTimeState(calculatedAmount, x, _turns)));
    }
}