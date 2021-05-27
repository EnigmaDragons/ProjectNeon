using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GlitchCards : Effect
{
    private readonly int _maxCards;
    private readonly CardLocation _location;
    private readonly Func<IEnumerable<Card>, IEnumerable<Card>> _orderBy;

    public GlitchCards(int maxCards, string location, Func<IEnumerable<Card>, IEnumerable<Card>> orderBy)
    {
        _maxCards = maxCards;
        _location = string.IsNullOrWhiteSpace(location) ? CardLocation.Nowhere : (CardLocation) int.Parse(location);
        _orderBy = orderBy;
    }
    
    public void Apply(EffectContext ctx)
    {
        List<Card> possibleCards = new List<Card>();
        if (_location.HasFlag(CardLocation.Hand))
            possibleCards.AddRange(ctx.PlayerCardZones.HandZone.Cards);
        if (_location.HasFlag(CardLocation.Deck))
            possibleCards.AddRange(ctx.PlayerCardZones.DrawZone.Cards);
        if (_location.HasFlag(CardLocation.Discard))
            possibleCards.AddRange(ctx.PlayerCardZones.DiscardZone.Cards);
        var filteredCards = possibleCards
            .Where(card => ctx.Target.Members.Any(m => card.Owner.Id == m.Id) && card.Mode != CardMode.Glitched)
            .ToArray()
            .Shuffled();
        var orderedCards = _orderBy(filteredCards).ToArray();
        for (var i = 0; i < _maxCards && i < orderedCards.Length; i++)
            orderedCards[i].TransitionTo(CardMode.Glitched);
    }
}