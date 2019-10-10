using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : ScriptableObject
{
    [SerializeField] private DeckBuilderState state;
    private Card current;

    public void Init (Card current)
    {
        this.current = current;
    }

    public void ListToDeck()
    {
        this.state.ListToDeck(current);
    }

    public void DeckToList()
    {
        this.state.DeckToList(current);
    }
}
