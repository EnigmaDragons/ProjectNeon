using UnityEngine;

public class CardCycled
{
    public Transform UiSource { get; }
    public Card CycledCard { get; }
    public Card DrawnCard { get; }

    public CardCycled(Transform uiSource, Card cycledCard, Card drawnCard)
    {
        UiSource = uiSource;
        CycledCard = cycledCard;
        DrawnCard = drawnCard;
    }
}
