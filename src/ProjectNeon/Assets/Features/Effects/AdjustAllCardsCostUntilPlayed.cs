public class AdjustAllCardsCostUntilPlayed : Effect
{
    private int _amount;

    public AdjustAllCardsCostUntilPlayed(int amount)
    {
        _amount = amount;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.PlayerCardZones.AllCards.ForEach(x => x.AddState(new GainStateIfInZoneAtEndOfTurn(new AdjustedCardState(-1, 1, _amount), x, ctx.PlayerCardZones.HandZone)));
    }
}