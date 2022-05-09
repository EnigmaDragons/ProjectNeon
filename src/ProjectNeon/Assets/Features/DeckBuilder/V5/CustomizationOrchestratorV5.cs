using UnityEngine;

public class CustomizationOrchestratorV5 : MonoBehaviour
{
    [SerializeField] private HeroSelectionUI heroSelection;
    [SerializeField] private DeckUI deckUI;
    
    private void OnEnable()
    {
        heroSelection.Init();
        deckUI.GenerateDeck();
    }
}
