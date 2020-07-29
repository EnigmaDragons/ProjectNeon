using UnityEngine;

public class BattleUiVisuals : MonoBehaviour
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    [SerializeField] private GameObject resolutionPhaseUi;
    [SerializeField] private GameObject hand;
    
    public void Setup()
    {
        HideCommandPhaseUI();
        HideResolutionPhaseUI();
        partyUi.Setup();
    }

    private void HideCommandPhaseUI() => commandPhaseUi.SetActive(false);
    public void BeginCommandPhase()
    {
        commandPhaseUi.SetActive(true);
        resolutionPhaseUi.SetActive(true);
    }

    public void EndCommandPhase() => HideCommandPhaseUI();

    public void BeginResolutionPhase()
    {
        resolutionPhaseUi.SetActive(true);
        hand.SetActive(false);
    }

    public void EndResolutionPhase()
    {
        HideResolutionPhaseUI();
        hand.SetActive(true);
    }

    private void HideResolutionPhaseUI() => resolutionPhaseUi.SetActive(false);
}
