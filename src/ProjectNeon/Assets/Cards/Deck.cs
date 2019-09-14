using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck 
{
    private List<Card> cards;

    Deck(List<Card> cards)
    {
        this.cards = cards;
    }

    void shuffle()
    {
        cards.Sort((a, b) => (int)System.Math.Round(Random.value, 0));
    }


}
