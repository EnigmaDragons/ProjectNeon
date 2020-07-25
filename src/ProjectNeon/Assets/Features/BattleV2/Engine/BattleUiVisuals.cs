using UnityEngine;

public class BattleUiVisuals : MonoBehaviour
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    
    public void Setup()
    {
        partyUi.Setup();
    }
}
