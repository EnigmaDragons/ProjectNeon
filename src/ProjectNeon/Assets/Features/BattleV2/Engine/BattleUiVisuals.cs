using UnityEngine;

public class BattleUiVisuals : MonoBehaviour
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    
    public void Setup()
    {
        partyUi.Setup();
        HideCommandPhaseUI();
    }

    private void HideCommandPhaseUI()
    {
        commandPhaseUi.SetActive(false);
    }

    public void BeginCommandPhase()
    {
        commandPhaseUi.SetActive(true);
    }

    public void EndCommandPhase()
    {
        HideCommandPhaseUI();
    }
}
