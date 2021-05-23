using UnityEngine;

public class ShowReferencedCard
{
    public Maybe<Card> Card { get; }
    public CardTypeData CardType { get; }
    public GameObject Parent { get; }

    public ShowReferencedCard(GameObject parent, Card card) : this(parent, card, card.Type) {}
    public ShowReferencedCard(GameObject parent, CardTypeData cardType) : this(parent, Maybe<Card>.Missing(), cardType) {}

    public ShowReferencedCard(GameObject parent, Maybe<Card> card, CardTypeData cardType)
    {
        Parent = parent;
        Card = card;
        CardType = cardType;
    }
}
