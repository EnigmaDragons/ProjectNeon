using UnityEngine;

public class CustomizationOrchestratorV4 : MonoBehaviour
{
    [SerializeField] private HeroSelectionUI heroSelection;
    [SerializeField] private LibraryFilterUI cardFilter;
    [SerializeField] private DeckUI deckUI;
    
    private void Awake()
    {
        heroSelection.Init();
        cardFilter.Regenerate();
        deckUI.GenerateDeck();
    }
}
