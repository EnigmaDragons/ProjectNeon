using UnityEngine;

public class CustomizationOrchestrator : MonoBehaviour
{
    [SerializeField] private EquipmentLibraryFilterUI equipmentFilter;
    [SerializeField] private LibraryFilterUI cardFilter;
    [SerializeField] private HeroDetailsPanelForCustomization heroDetails;
    
    private void Awake()
    {
        equipmentFilter.Regenerate();
        cardFilter.Regenerate();
        heroDetails.Initialized();
    }
}