using UnityEngine;

public class CardsCycled
{
    public Card[] CycledCards { get; }
    public Card[] DrawnCards { get; }

    public CardsCycled(Card[] cycledCards, Card[] drawnCards)
    {
        CycledCards = cycledCards;
        DrawnCards = drawnCards;
    }
}