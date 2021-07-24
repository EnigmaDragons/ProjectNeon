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
    
    public bool ReadyForTurnEnd => _confirmRequested || battleState.NumberOfCardPlaysRemainingThisTurn <= 0 || battleState.PlayerCardZones.HandZone.Count == 0;

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
        playerHand.OnZoneCardsChanged.Subscribe(UpdateState, this);
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
        if (_isConfirming == ReadyForTurnEnd)
            return;
        
        _isConfirming = ReadyForTurnEnd;
        if (_isConfirming)
            Message.Publish(new PlayerTurnConfirmationStarted());
        else
            Message.Publish(new PlayerTurnConfirmationAborted());
        
        if (!battleState.HasMorePlaysAvailableThisTurn)
            Confirm();
    }

    private void ConfirmEarly()
    {
        _confirmRequested = true;
        Confirm();
    }
    
    public void Confirm()
    {
        if (!ReadyForTurnEnd) 
            return;

        var reasonWord = _confirmRequested ? "manually" : "automatically";
        _confirmRequested = false;
        confirmUi.gameObject.SetActive(false);
        playArea.Clear();
        if (battleState.Phase != BattleV2Phase.NotBegun)
            BattleLog.Write($"Turn {battleState.TurnNumber} {battleState.Phase} - Player turn ended {reasonWord}");
        Message.Publish(new PlayerTurnConfirmed());
    }

    public void Cancel()
    {
        _confirmRequested = false;
        UpdateState();
    }
}
