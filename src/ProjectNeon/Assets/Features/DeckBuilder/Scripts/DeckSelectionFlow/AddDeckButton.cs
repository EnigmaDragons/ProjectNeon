using System.Collections.Generic;
using UnityEngine;

public class AddDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;

    private DeckBuilderNavigation _navigation;

    public void Init(DeckBuilderNavigation navigation) => _navigation = navigation;

    public void AddDeck()
    {
        state.Operation = DeckBuilderOperation.Add;
        state.SelectedDeck = new Deck { Name = "", ClassName = state.SelectedHero.ClassName, Cards = new List<Card>(), IsImmutable = false };
        _navigation.NavigateToDeckBuilder();
    }
}
