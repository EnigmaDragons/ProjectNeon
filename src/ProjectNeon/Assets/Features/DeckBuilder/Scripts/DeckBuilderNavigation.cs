using UnityEngine;

public class DeckBuilderNavigation : MonoBehaviour
{
    [SerializeField] private GameObject characterSelection;
    [SerializeField] private GameObject deckSelection;
    [SerializeField] private GameObject deckBuilder;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent heroSelectionRequired;
    [SerializeField] private GameEvent deckSelectionRequired;
    [SerializeField] private GameEvent deckBuildingStarted;

    private void OnEnable()
    {
        heroSelectionRequired.Subscribe(NavigateToHeroSelection, this);
        deckSelectionRequired.Subscribe(NavigateToDeckSelection, this);
        deckBuildingStarted.Subscribe(NavigateToDeckBuilder, this);
    }

    private void OnDisable()
    {
        heroSelectionRequired.Unsubscribe(this);
        deckSelectionRequired.Unsubscribe(this);
        deckBuildingStarted.Unsubscribe(this);
    }

    private void NavigateToHeroSelection()
    {
        characterSelection.SetActive(true);
        deckSelection.SetActive(false);
        deckBuilder.SetActive(false);
    }

    private void NavigateToDeckSelection()
    {
        state.DeckIsSelected = false;
        characterSelection.SetActive(false);
        deckSelection.SetActive(true);
        deckBuilder.SetActive(false);
    }

    private void NavigateToDeckBuilder()
    {
        state.TemporaryDeck = state.SelectedDeck.Export();
        characterSelection.SetActive(false);
        deckSelection.SetActive(false);
        deckBuilder.SetActive(true);
    }
}
