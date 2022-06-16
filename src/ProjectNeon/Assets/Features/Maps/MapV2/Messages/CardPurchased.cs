using UnityEngine;

public class CardPurchased 
{
    public Transform UiSource { get; }
    public CardTypeData Card { get; }

    public CardPurchased(CardTypeData card, Transform uiSource)
    {
        Card = card;
        UiSource = uiSource;
    }
}
