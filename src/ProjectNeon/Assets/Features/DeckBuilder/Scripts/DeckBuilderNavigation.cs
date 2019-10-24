using UnityEngine;

public class DeckBuilderNavigation : MonoBehaviour
{
    [SerializeField] private GameObject characterSelection;
    [SerializeField] private GameObject deckSelection;
    [SerializeField] private GameObject deckBuilder;
    [SerializeField] private DeckBuilderState state;

    public void NavigateToHeroSelection()
    {
        characterSelection.SetActive(true);
        deckSelection.SetActive(false);
        deckBuilder.SetActive(false);
    }

    public void NavigateToDeckSelection()
    {
        state.DeckIsSelected = false;
        characterSelection.SetActive(false);
        deckSelection.SetActive(true);
        deckBuilder.SetActive(false);
    }

    public void NavigateToDeckBuilder()
    {
        state.TemporaryDeck = state.SelectedDeck.Export();
        characterSelection.SetActive(false);
        deckSelection.SetActive(false);
        deckBuilder.SetActive(true);
    }
}
