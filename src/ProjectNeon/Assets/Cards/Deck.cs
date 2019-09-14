using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    private List<Card> cards;

    Deck(List<Card> cards)
    {
        this.cards = cards;
    }

    void Shuffle()
    {
        cards.Sort((a, b) => (int)System.Math.Round(Random.value, 0));
    }

}
