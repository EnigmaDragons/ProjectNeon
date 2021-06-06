using System.Collections;
using UnityEngine;

public class BattleUiVisuals : OnMessage<BattleFinished, TargetSelectionBegun, TargetSelectionFinished, PlayerCardCanceled, CardResolutionStarted, CardResolutionFinished>
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private ResolutionZoneOffset resolutionPhaseUi;
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject defeatUi;
    [SerializeField] private GameObject victoryUi;
    
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardResolutionZone playArea;
    
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
        hand.SetActive(battleState.Phase == BattleV2Phase.PlayCards && battleState.HasMorePlaysAvailableThisTurn);
    }
}
