
public class ShowDetailedCardView
{
    public Maybe<Card> Card { get; }
    public CardTypeData CardType { get; }

    public ShowDetailedCardView(Card card) : this(card, card.Type) {}
    public ShowDetailedCardView(CardTypeData cardType) : this(Maybe<Card>.Missing(), cardType) {}

    private ShowDetailedCardView(Maybe<Card> card, CardTypeData cardType)
    {
        Card = card;
        CardType = cardType;
    }
}
