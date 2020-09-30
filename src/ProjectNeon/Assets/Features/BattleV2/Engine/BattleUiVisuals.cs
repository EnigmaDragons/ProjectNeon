using System.Collections;
using UnityEngine;

public class BattleUiVisuals : OnMessage<BattleFinished>
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    [SerializeField] private ResolutionZoneOffset resolutionPhaseUi;
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject defeatUi;
    [SerializeField] private GameObject victoryUi;
    
    public void Setup()
    {
        HideCommandPhaseUI();
        HideResolutionPhaseUI();
        partyUi.Setup();
    }

    private void HideCommandPhaseUI() => commandPhaseUi.SetActive(false);
    public void BeginCommandPhase()
    {
        hand.SetActive(true);
        commandPhaseUi.SetActive(true);
        resolutionPhaseUi.gameObject.SetActive(true);
    }

    public void EndCommandPhase()
    {
        HideCommandPhaseUI();
        hand.SetActive(false);
    }

    public IEnumerator BeginResolutionPhase()
    {
        resolutionPhaseUi.gameObject.SetActive(true);
        yield return resolutionPhaseUi.BeginResolutionPhase();
    }

    public void EndResolutionPhase()
    {
        resolutionPhaseUi.EndResolutionPhase();
        HideResolutionPhaseUI();
        hand.SetActive(false);
    }

    private void HideResolutionPhaseUI() => resolutionPhaseUi.gameObject.SetActive(false);
    
    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Enemies)
            defeatUi.SetActive(true);
        else
            victoryUi.SetActive(true);
    }
}
