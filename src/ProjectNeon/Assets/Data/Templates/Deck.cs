using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField]
    private List<Card> cards;

    public void Shuffle()
    {
    }

    public List<Card> GetCards()
    {
        return this.cards;
    }

}
