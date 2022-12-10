
using System;
using UnityEngine;

public class PlayerEffect : Effect
{
    private readonly Action<int, PlayerState, int, int> _action;
    private readonly string _durationFormula;
    private readonly string _formula;

    public PlayerEffect(Action<int, PlayerState, int> action, string durationFormula)
        : this((id, state, duration, amount) => action(id, state, duration), durationFormula, "0") {}
    public PlayerEffect(Action<PlayerState, int, int> action, string durationFormula, string formula)
        : this((id, state, duration, amount) => action(state, duration, amount), durationFormula, formula) {}
    private PlayerEffect(Action<int, PlayerState, int, int> action, string durationFormula, string formula)
    {
        _action = action;
        _durationFormula = durationFormula;
        _formula = formula;
    }
    
    public void Apply(EffectContext ctx)
    {
        _action(ctx.Source.Id, ctx.PlayerState, 
            Formula.EvaluateToInt(ctx.SourceSnapshot.State, _durationFormula, ctx.XPaidAmount, ctx.ScopedData),
            Formula.EvaluateToInt(ctx.SourceSnapshot.State, _formula, ctx.XPaidAmount, ctx.ScopedData));
    }
}
