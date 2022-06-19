using UnityEngine;

public class BattleUiVisuals : OnMessage<BattleFinished, TargetSelectionBegun, TargetSelectionFinished, PlayerCardCanceled, CardResolutionStarted, CardResolutionFinished>
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
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

    private void HideCommandPhaseUI() {}
    public void BeginCommandPhase()
    {
        hand.SetActive(true);
        if (resolutionPhaseUi != null)
            resolutionPhaseUi.gameObject.SetActive(true);
    }

    public void EndCommandPhase()
    {
        HideCommandPhaseUI();
    }
    
    private void HideResolutionPhaseUI()
    {
        if (resolutionPhaseUi != null)
            resolutionPhaseUi.gameObject.SetActive(false);
    }

    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Enemies)
            defeatUi.SetActive(true);
        else if (victoryUi != null)
            victoryUi.SetActive(true);
    }

    protected override void Execute(TargetSelectionBegun msg) {}
    protected override void Execute(TargetSelectionFinished msg) => RefreshHandVisibility();
    protected override void Execute(PlayerCardCanceled msg) => RefreshHandVisibility();
    protected override void Execute(CardResolutionStarted msg) => RefreshHandVisibility();
    protected override void Execute(CardResolutionFinished msg) => RefreshHandVisibility();

    private void RefreshHandVisibility()
    {
        hand.SetActive(true);
    }
}
