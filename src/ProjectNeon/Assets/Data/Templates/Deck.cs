using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField]
    private List<Card> cards;
    public void Shuffle()
    {
        /**
         * @todo #54:30min We should compose deck shuffling behavior. Create a script 
         * that shuffles the deck and add it as an object to deck
         */
    }

}
