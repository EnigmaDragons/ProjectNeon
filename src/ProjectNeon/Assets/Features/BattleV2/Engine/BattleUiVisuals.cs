using UnityEngine;

public class BattleUiVisuals : OnMessage<BattleFinished>
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    [SerializeField] private GameObject resolutionPhaseUi;
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
    
    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Enemies)
            defeatUi.SetActive(true);
        else
            victoryUi.SetActive(true);
    }
}
