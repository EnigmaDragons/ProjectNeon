using UnityEngine;

public class SwappedCard
{
    public Transform UiSource { get; }
    public Card Card { get; }
    
    public SwappedCard(Transform uiSource, Card card)
    {
        UiSource = uiSource;
        Card = card;
    }
}
