using UnityEngine;

public class BattleUiVisuals : MonoBehaviour
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject turnConfirmation;
    [SerializeField] private GameObject handArea;
    [SerializeField] private GameObject targetSelection;
    
    public void Setup()
    {
        partyUi.Setup();
    }

    public void BeginCommandPhase()
    {
        turnConfirmation.gameObject.SetActive(true);
        targetSelection.gameObject.SetActive(true);
    }

    public void EndCommandPhase()
    {
        turnConfirmation.gameObject.SetActive(false);
        targetSelection.gameObject.SetActive(false);
    }
}
