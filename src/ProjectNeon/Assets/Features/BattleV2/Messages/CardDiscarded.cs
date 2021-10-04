using UnityEngine;

public class CardDiscarded
{
    public Transform UiSource { get; }
    public Card Card { get; }

    public CardDiscarded(Transform uiSource, Card card)
    {
        UiSource = uiSource;
        Card = card;
    }
}
