using UnityEngine;

public class CustomizationOrchestrator : MonoBehaviour
{
    [SerializeField] private HeroSelectionUI heroSelection;
    [SerializeField] private SelectHeroButtonV2[] selectHeroButtons;
    [SerializeField] private EquipmentLibraryFilterUI equipmentFilter;
    [SerializeField] private LibraryFilterUI cardFilter;
    [SerializeField] private HeroDetailsPanelForCustomization heroDetails;
    
    private void Awake()
    {
        heroSelection.Init();
        selectHeroButtons.ForEach(x => x.Init());
        equipmentFilter.Regenerate();
        cardFilter.Regenerate();
        heroDetails.Initialized();
    }
}