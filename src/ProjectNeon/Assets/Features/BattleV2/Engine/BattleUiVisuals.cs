using UnityEngine;

public class BattleUiVisuals : MonoBehaviour
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    [SerializeField] private GameObject resolutionPhaseUi;
    
    public void Setup()
    {
        HideCommandPhaseUI();
        HideResolutionPhaseUI();
        partyUi.Setup();
    }

    private void HideCommandPhaseUI() => commandPhaseUi.SetActive(false);
    public void BeginCommandPhase() => commandPhaseUi.SetActive(true);
    public void EndCommandPhase() => HideCommandPhaseUI();

    public void BeginResolutionPhase() => resolutionPhaseUi.SetActive(true);
    public void EndResolutionPhase() => HideResolutionPhaseUI();
    private void HideResolutionPhaseUI() => resolutionPhaseUi.SetActive(false);
}
