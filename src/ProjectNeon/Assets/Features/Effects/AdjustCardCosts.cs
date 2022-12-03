﻿
public class AdjustCardCosts : Effect
{
    private readonly CardLocation _cardLocation;
    private readonly string _formula;

    public AdjustCardCosts(string effectScope, string formula)
    {
        _cardLocation = string.IsNullOrWhiteSpace(effectScope) ? CardLocation.Nowhere : (CardLocation) int.Parse(effectScope);
        _formula = formula;
    }
    
    public void Apply(EffectContext ctx)
    {
        if (_cardLocation.HasFlag(CardLocation.Hand))
            ctx.PlayerCardZones.HandZone.Cards.ForEach(x => x.AddState(new AdjustedCardState(-1, 1, Formula.EvaluateToInt(ctx.SourceSnapshot.State, _formula, ctx.XPaidAmount, ctx.ScopedData))));
        if (_cardLocation.HasFlag(CardLocation.Deck))
            ctx.PlayerCardZones.DrawZone.Cards.ForEach(x => x.AddState(new AdjustedCardState(-1, 1, Formula.EvaluateToInt(ctx.SourceSnapshot.State, _formula, ctx.XPaidAmount, ctx.ScopedData))));
        if (_cardLocation.HasFlag(CardLocation.Discard))
            ctx.PlayerCardZones.DiscardZone.Cards.ForEach(x => x.AddState(new AdjustedCardState(-1, 1, Formula.EvaluateToInt(ctx.SourceSnapshot.State, _formula, ctx.XPaidAmount, ctx.ScopedData))));
    }
}
