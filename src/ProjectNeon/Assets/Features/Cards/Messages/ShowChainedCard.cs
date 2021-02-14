using UnityEngine;

public class ShowChainedCard
{
    public Maybe<Card> Card { get; }
    public CardTypeData CardType { get; }
    public GameObject Parent { get; }

    public ShowChainedCard(GameObject parent, Card card) : this(parent, card, card.Type) {}
    public ShowChainedCard(GameObject parent, CardTypeData cardType) : this(parent, Maybe<Card>.Missing(), cardType) {}

    public ShowChainedCard(GameObject parent, Maybe<Card> card, CardTypeData cardType)
    {
        Parent = parent;
        Card = card;
        CardType = cardType;
    }
}
