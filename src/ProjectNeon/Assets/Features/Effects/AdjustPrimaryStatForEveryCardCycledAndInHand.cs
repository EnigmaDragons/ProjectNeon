﻿using UnityEngine;

public class AdjustPrimaryStatForEveryCardCycledAndInHand : Effect
{
    private readonly EffectData _e;

    public AdjustPrimaryStatForEveryCardCycledAndInHand(EffectData e)
    {
        _e = e;
    }
    
    public void Apply(EffectContext ctx)
    {
        var amount = ctx.PlayerCardZones.HandZone.Count + ctx.PlayerState.NumberOfCyclesUsedThisTurn;
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().With(m.PrimaryStat(), amount), 
            _e.ForSimpleDurationStatAdjustment(ctx.Source.Id, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, _e.DurationFormula, ctx.XPaidAmount, ctx.ScopedData)))));
    }
}