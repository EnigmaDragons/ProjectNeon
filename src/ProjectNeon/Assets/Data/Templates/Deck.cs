using System.Collections.Generic;
using UnityEngine;

public class Deck : ScriptableObject
{
    public string Name;
    public StringVariable ClassName;
    public List<Card> Cards;
    public bool IsImmutable;

    public Deck Export()
    {
        return Instantiate(this);
    }

    public void Import(Deck deck)
    {
        Name = deck.Name;
        ClassName = deck.ClassName;
        Cards = deck.Cards;
        IsImmutable = false;
    }
}
