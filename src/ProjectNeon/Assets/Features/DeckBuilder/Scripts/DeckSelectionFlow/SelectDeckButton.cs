using TMPro;
using UnityEngine;

public class SelectDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI deckNameText;

    private DeckBuilderNavigation _navigation;
    private Deck _deck;

    public void Init(DeckBuilderNavigation navigation, Deck deck)
    {
        _navigation = navigation;
        _deck = deck;
        deckNameText.text = deck.Name;
    }

    public void Select()
    {
        if (state.DeckIsSelected && state.SelectedDeck == _deck)
        {
            state.Operation = state.SelectedDeck.IsImmutable ? DeckBuilderOperation.View : DeckBuilderOperation.Edit;
            _navigation.NavigateToDeckBuilder();
        }
        else
        {
            state.SelectedDeck = _deck;
            state.DeckIsSelected = true;
        }
    }
}
