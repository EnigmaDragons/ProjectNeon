using System.Collections.Generic;
using System.Linq;

public class DrawSelectedCard : Effect
{
    private readonly CardLocation _location;

    public DrawSelectedCard(string location)
    {
        _location = string.IsNullOrWhiteSpace(location) ? CardLocation.Nowhere : (CardLocation) int.Parse(location);
    }
    
    public void Apply(EffectContext ctx)
    {
        List<Card> possibleCards = new List<Card>();
        if (_location.HasFlag(CardLocation.Deck))
            possibleCards.AddRange(ctx.PlayerCardZones.DrawZone.Cards);
        if (_location.HasFlag(CardLocation.Discard))
            possibleCards.AddRange(ctx.PlayerCardZones.DiscardZone.Cards);
        ctx.Selections.CardSelectionOptions = possibleCards.Shuffled().ToArray();
        ctx.Selections.OnCardSelected = card =>
        {
            if (ctx.PlayerCardZones.DiscardZone.Cards.Contains(card))
                ctx.PlayerCardZones.DiscardZone.Remove(card);
            if (ctx.PlayerCardZones.DrawZone.Cards.Contains(card))
                ctx.PlayerCardZones.DrawZone.Remove(card);
            ctx.PlayerCardZones.HandZone.PutOnBottom(card);
        };
    }
}