using UnityEngine;

public class DeckBuilderNavigation : MonoBehaviour
{
    [SerializeField] private GameObject deckSelection;
    [SerializeField] private GameObject deckBuilder;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckSelectionRequired;
    [SerializeField] private GameEvent deckBuildingStarted;

    private void OnEnable()
    {
        deckSelectionRequired.Subscribe(NavigateToDeckSelection, this);
        deckBuildingStarted.Subscribe(NavigateToDeckBuilder, this);
    }

    private void OnDisable()
    {
        deckSelectionRequired.Unsubscribe(this);
        deckBuildingStarted.Unsubscribe(this);
    }

    private void NavigateToDeckSelection()
    {
        state.DeckIsSelected = false;
        deckSelection.SetActive(true);
        deckBuilder.SetActive(false);
    }

    private void NavigateToDeckBuilder()
    {
        state.TemporaryDeck = state.SelectedDeck.Export();
        deckSelection.SetActive(false);
        deckBuilder.SetActive(true);
    }
}
