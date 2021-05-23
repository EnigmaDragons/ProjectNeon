
using System;
using UnityEngine;

public class PlayerEffect : Effect
{
    private readonly Action<PlayerState, int> _action;
    private readonly string _durationFormula;

    public PlayerEffect(Action<PlayerState, int> action, string durationFormula)
    {
        _action = action;
        _durationFormula = durationFormula;
    }
    
    public void Apply(EffectContext ctx)
    {
        _action(ctx.PlayerState, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, _durationFormula, ctx.XPaidAmount)));
    }
}
