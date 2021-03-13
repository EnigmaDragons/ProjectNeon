using System;
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
    
    private bool _isResolvingInstantCard;
    [Obsolete] private bool HasMoreCardPlays => playArea.NumPlayedThisTurn < battleState.PlayerState.CurrentStats.CardPlays();
    
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
    protected override void Execute(CardResolutionStarted msg)
    {
        if (!msg.Card.IsInstant()) 
            return;
        
        _isResolvingInstantCard = true;
        RefreshHandVisibility();
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        if (!msg.CardWasInstant) 
            return;
        
        _isResolvingInstantCard = false;
        RefreshHandVisibility();
    }

    private void RefreshHandVisibility()
    {
        Debug.Log($"Refresh Hand Visibility. {playArea.NumPlayedThisTurn} / {battleState.PlayerState.CurrentStats.CardPlays()} Card Is Instant: {_isResolvingInstantCard}");
        hand.SetActive(!_isResolvingInstantCard && HasMoreCardPlays);
    }
}
