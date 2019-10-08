using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Card current;

    public void ListToDeck()
    {
        this.state.ListToDeck(current);
    }

    public void DeckToList()
    {
        this.state.DeckToList(current);
    }
}
