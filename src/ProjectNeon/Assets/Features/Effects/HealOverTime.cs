using System;
using UnityEngine;

public sealed class HealOverTime : Effect
{
    private string _turnsFormula;
    private float _multiplier;

    public HealOverTime(float multiplier, string turnsFormula) {
        _turnsFormula = turnsFormula;
        _multiplier = multiplier;
    }
    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(
            new HealOverTimeState(ctx.Source.Id, (int)Math.Ceiling(_multiplier * ctx.Source.Magic()), x, 
                Formula.EvaluateToInt(ctx.SourceSnapshot.State, x.State, _turnsFormula, ctx.XPaidAmount))));
    }
}
