using UnityEngine;
using UnityEngine.UI;

public class ConfirmPlayerTurnV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private Button confirmUi;

    private bool _isConfirming = false;
    private bool _confirmRequested;
    
    public bool CanConfirm => playArea.Cards.Length == battleState.PlayerState.CurrentStats.CardPlays() || _confirmRequested;

    private void Awake()
    {
        confirmUi.gameObject.SetActive(false);
        confirmUi.onClick.AddListener(ConfirmEarly);
    }
    
    private void OnEnable()
    {
        Message.Subscribe<BattleStateChanged>(msg => UpdateState(msg), this);
        Message.Subscribe<BeginPlayerTurnConfirmation>(_ => OnConfirmationRequested(), this);
        playArea.OnZoneCardsChanged.Subscribe(UpdateState, this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
        playArea.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void OnConfirmationRequested()
    {
        if (battleState.Phase != BattleV2Phase.PlayCards || battleState.IsSelectingTargets || playArea.Cards.Length != battleState.PlayerState.CurrentStats.CardPlays())
            return;
        ConfirmEarly();
    }

    private void UpdateState(BattleStateChanged msg)
    {
        if (msg.Before.Phase != BattleV2Phase.PlayCards && battleState.Phase == BattleV2Phase.PlayCards)
            confirmUi.gameObject.SetActive(true);
        if (battleState.Phase != BattleV2Phase.PlayCards)
            confirmUi.gameObject.SetActive(false);
        UpdateState();
    }
    
    private void UpdateState()
    {
        if (_isConfirming == CanConfirm)
            return;
        
        _isConfirming = CanConfirm;
        if (_isConfirming)
            Message.Publish(new PlayerTurnConfirmationStarted());
        else
            Message.Publish(new PlayerTurnConfirmationAborted());
        
        if (battleState.NumberOfCardPlaysRemainingThisTurn == 0 
                && battleState.NumberOfRecyclesRemainingThisTurn == 0 
                && playerHand.Cards.None(x => x.TimingType == CardTimingType.Instant))
            Confirm();
    }

    private void ConfirmEarly()
    {
        _confirmRequested = true;
        confirmUi.gameObject.SetActive(false);
        Confirm();
    }
    
    public void Confirm()
    {
        if (!CanConfirm) return;

        _confirmRequested = false;
        playArea.Clear();
        DevLog.Write("Player Confirmed Turn");
        Message.Publish(new PlayerTurnConfirmed());
    }

    public void Cancel()
    {
        _confirmRequested = false;
        UpdateState();
    }
}
