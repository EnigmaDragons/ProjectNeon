using UnityEngine;

public class CustomizationOrchestrator : MonoBehaviour
{
    [SerializeField] private HeroSelectionUI heroSelection;
    [SerializeField] private EquipmentLibraryFilterUI equipmentFilter;
    [SerializeField] private LibraryFilterUI cardFilter;
    [SerializeField] private HeroDetailsPanelForCustomization heroDetails;
    [SerializeField] private DeckUI deckUI;
    
    private void Awake()
    {
        heroSelection.Init();
        equipmentFilter.Regenerate();
        cardFilter.Regenerate();
        heroDetails.Initialized();
        deckUI.GenerateDeck();
    }
}