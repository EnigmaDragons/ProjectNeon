using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    public List<Card> Cards;

    public Deck Export()
    {
        return Instantiate(this);
    }

    public void Import(Deck deck)
    {
        Cards = deck.Cards;
    }
}
