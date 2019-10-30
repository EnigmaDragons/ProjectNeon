using TMPro;
using UnityEngine;

public class SelectDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI deckNameText;
    [SerializeField] private GameEvent deckBuildingStarted;
    
    private Deck _deck;

    public void Init(Deck deck)
    {
        _deck = deck;
        deckNameText.text = deck.Name;
    }

    public void Select()
    {
        if (state.DeckIsSelected && state.SelectedDeck == _deck)
        {
            state.Operation = state.SelectedDeck.IsImmutable ? DeckBuilderOperation.View : DeckBuilderOperation.Edit;
            deckBuildingStarted.Publish();
        }
        else
        {
            state.SelectedDeck = _deck;
            state.DeckIsSelected = true;
        }
    }
}
