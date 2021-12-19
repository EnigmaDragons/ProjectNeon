
using System;
using UnityEngine;

public class PlayerEffect : Effect
{
    private readonly Action<PlayerState, int, int> _action;
    private readonly string _durationFormula;
    private readonly string _formula;

    public PlayerEffect(Action<PlayerState, int, int> action, string durationFormula, string formula)
    {
        _action = action;
        _durationFormula = durationFormula;
        _formula = formula;
    }
    
    public void Apply(EffectContext ctx)
    {
        _action(ctx.PlayerState, 
            Formula.EvaluateToInt(ctx.SourceSnapshot.State, _durationFormula, ctx.XPaidAmount),
            Formula.EvaluateToInt(ctx.SourceSnapshot.State, _formula, ctx.XPaidAmount));
    }
}
