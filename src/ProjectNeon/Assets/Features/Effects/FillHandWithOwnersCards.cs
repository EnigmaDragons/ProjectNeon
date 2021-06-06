public class FillHandWithOwnersCards : Effect
{
    public void Apply(EffectContext ctx)
    {
        var targetId = ctx.Target.Members[0].Id;
        foreach (var card in ctx.PlayerCardZones.HandZone.Cards)
        {
            if (card.Owner.Id != targetId)
            {
                ctx.PlayerCardZones.HandZone.Remove(card);
                ctx.PlayerCardZones.DiscardZone.PutOnBottom(card);
            }
        }
        while (!ctx.PlayerCardZones.HandZone.IsFull && ctx.PlayerCardZones.HandZone.Cards.Length < ctx.PlayerState.CardDraws)
        {
            var first = ctx.PlayerCardZones.DrawZone.Cards.FirstOrMaybe(x => x.Owner.Id == targetId);
            if (first.IsPresent)
            {
                ctx.PlayerCardZones.DrawZone.Remove(first.Value);
                ctx.PlayerCardZones.HandZone.PutOnBottom(first.Value);
            }
            else if (ctx.PlayerCardZones.DiscardZone.HasCards)
            {
                ctx.PlayerCardZones.Reshuffle();
            }
            else
            {
                Log.Error($"Couldn't draw cards for {ctx.Target.Members[0].Name}");
                break;
            }
        }
    }
}