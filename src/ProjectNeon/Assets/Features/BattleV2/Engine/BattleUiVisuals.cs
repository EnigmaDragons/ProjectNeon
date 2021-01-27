using System.Collections;
using UnityEngine;

public class BattleUiVisuals : OnMessage<BattleFinished, TargetSelectionBegun, TargetSelectionFinished, PlayerCardCanceled, CardResolutionStarted, CardResolutionFinished>
{
    [SerializeField] private PartyUiSummaryV2 partyUi;
    [SerializeField] private GameObject commandPhaseUi;
    [SerializeField] private ResolutionZoneOffset resolutionPhaseUi;
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject defeatUi;
    [SerializeField] private GameObject victoryUi;
    
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardResolutionZone playArea;
    
    private bool _isResolvingInstantCard;
    private bool HasMoreCardPlays => playArea.Count < battleState.PlayerState.CurrentStats.CardPlays();
    
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
        else if (victoryUi != null)
            victoryUi.SetActive(true);
    }

    protected override void Execute(TargetSelectionBegun msg) => hand.SetActive(false);
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
        Debug.Log($"Refresh Hand Visibility. {playArea.Count} / {battleState.PlayerState.CurrentStats.CardPlays()} Instant: {_isResolvingInstantCard}");
        hand.SetActive(!_isResolvingInstantCard && HasMoreCardPlays);
    }
}
