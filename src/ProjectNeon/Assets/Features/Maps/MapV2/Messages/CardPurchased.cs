using UnityEngine;

public class CardPurchased 
{
    public Transform UiSource { get; }
    public Card Card { get; }

    public CardPurchased(Card card, Transform uiSource)
    {
        Card = card;
        UiSource = uiSource;
    }
}
