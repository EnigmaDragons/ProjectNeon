using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField]
    private List<Card> cards; 

    public void Shuffle()
    {
    }

    public List<Card> GetCards
    {
        get {
            List<Card> ret = new List<Card>();
            cards.ForEach(
                card => ret.Add(card)
              );
            return ret;
        }
    }

}
