using System.Linq;

public class TransformOwnersCardsIntoCard : Effect
{
    private readonly int[] _cards;
    private readonly int _cardId;

    public TransformOwnersCardsIntoCard(string effectScope)
    {
        var split = effectScope.Split(',');
        _cardId = int.TryParse(split[0], out var cardId) ? cardId : -1;
        _cards = split.Skip(1).Select(int.Parse).ToArray();
    }

    public void Apply(EffectContext ctx)
    {
        var card = ctx.AllCards[_cardId];
        ReplaceZone(ctx, ctx.PlayerCardZones.HandZone, card);
        ReplaceZone(ctx, ctx.PlayerCardZones.DrawZone, card);
        ReplaceZone(ctx, ctx.PlayerCardZones.DiscardZone, card);
    }

    private void ReplaceZone(EffectContext ctx, CardPlayZone zone, CardTypeData cardReplacer)
    {
        while (zone.Cards.Any(card 
            => _cards.Contains(card.Id)
            && card.Owner.Id == ctx.Source.Id))
        {
            var cardToReplace = zone.Cards.First(card 
                => _cards.Contains(card.Id)
                && card.Owner.Id == ctx.Source.Id);
            zone.Replace(cardToReplace, new Card(ctx.GetNextCardId(), ctx.Source, cardReplacer));
        }
    }
}