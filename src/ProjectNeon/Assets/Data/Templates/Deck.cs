using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField]
    private List<Card> cards;

    public Deck(List<Card> cards)
    {
        this.cards = cards;
    }

    public void Shuffle()
    {
    }

}
