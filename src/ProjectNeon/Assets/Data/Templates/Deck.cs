using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField]
    private List<Card> cards; 

    public void Shuffle()
    {
    }

    public List<Card> Cards
    {
        get {
            return cards.ToList();
        }
    }

}
