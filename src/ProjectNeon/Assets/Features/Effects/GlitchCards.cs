using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GlitchCards : Effect
{
    private readonly int _maxCards;
    private readonly CardLocation _location;

    public GlitchCards(int maxCards, string location)
    {
        _maxCards = maxCards;
        _location = string.IsNullOrWhiteSpace(location) ? CardLocation.Nowhere : (CardLocation) int.Parse(location);
    }
    
    public void Apply(EffectContext ctx)
    {
        if (ctx.Target.Members.All(x => x.Aegis() > 0))
        {
            ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, ctx.Target.Members);   
            BattleLog.Write($"{string.Join(" & ", ctx.Target.Members.Select(x => x.Name))} prevented glitching with an Aegis");
            return;
        }
        
        var possibleCards = new List<CardGlitchedDetails>();
        if (_location.HasFlag(CardLocation.Hand))
            possibleCards.AddRange(ctx.PlayerCardZones.HandZone.Cards.Select(c => new CardGlitchedDetails(c, CardLocation.Hand)));
        if (_location.HasFlag(CardLocation.Deck))
            possibleCards.AddRange(ctx.PlayerCardZones.DrawZone.Cards.Select(c => new CardGlitchedDetails(c, CardLocation.Deck)));
        if (_location.HasFlag(CardLocation.Discard))
            possibleCards.AddRange(ctx.PlayerCardZones.DiscardZone.Cards.Select(c => new CardGlitchedDetails(c, CardLocation.Discard)));
        var filteredCards = possibleCards
            .Where(c => ctx.Target.Members.Any(m => c.Card.Owner.Id == m.Id) && c.Card.Mode != CardMode.Glitched)
            .ToArray()
            .Shuffled();
        var impactedCards = filteredCards.Take(Math.Min(_maxCards, filteredCards.Length)).ToArray();
        impactedCards.ForEach(c => c.Card.TransitionTo(CardMode.Glitched));
        Message.Publish(new CardsGlitched(impactedCards));
    }
}
