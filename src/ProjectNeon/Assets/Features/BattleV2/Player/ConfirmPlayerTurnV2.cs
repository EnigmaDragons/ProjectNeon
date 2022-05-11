using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPlayerTurnV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private Button confirmUi;

    private bool _isConfirming = false;
    private bool _confirmRequestedManually;
    private bool _alreadyConfirmedThisTurn = false;

    private void Awake()
    {
        confirmUi.gameObject.SetActive(false);
        confirmUi.onClick.AddListener(ConfirmEarly);
    }
    
    private void OnEnable()
    {
        Message.Subscribe<BattleStateChanged>(msg => UpdateState(msg), this);
        Message.Subscribe<BeginPlayerTurnConfirmation>(_ => OnConfirmationRequested(), this);
        Message.Subscribe<CheckForAutomaticTurnEnd>(msg => CheckForAutomaticTurnEnd(), this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
        playArea.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void OnConfirmationRequested()
    {
        if (battleState.Phase != BattleV2Phase.PlayCards || battleState.IsSelectingTargets)
            return;
        ConfirmEarly();
    }

    private void CheckForAutomaticTurnEnd()
    {
        if (battleState.Phase == BattleV2Phase.PlayCards && 
            (!battleState.PlayerCardZones.HandZone.HasCards 
                || (battleState.NumberOfRecyclesRemainingThisTurn <= 0 
                    && battleState.NumberOfCardPlaysRemainingThisTurn <= 0
                    && battleState.PlayerCardZones.PlayZone.IsEmpty
                    && battleState.PlayerCardZones.ResolutionZone.IsEmpty
                    && battleState.PlayerCardZones.HandZone.Cards.All(c => 
                        !c.IsAnyFormPlayableByHero(battleState.Party, battleState.NumberOfCardPlaysRemainingThisTurn) 
                        || !c.Owner.CanPlayCards()))))
        {
            DevLog.Write($"No playable cards. Requesting early turn Confirmation. Hand Size {battleState.PlayerCardZones.HandZone.Cards.Length}. Num Cycles {battleState.NumberOfRecyclesRemainingThisTurn}");
            Confirm();
        }
    }

    private void UpdateState(BattleStateChanged msg)
    {
        if (battleState.Phase == BattleV2Phase.PlayCards && msg.Before.Phase != BattleV2Phase.PlayCards)
            _alreadyConfirmedThisTurn = false;
        if (msg.Before.Phase != BattleV2Phase.PlayCards && battleState.Phase == BattleV2Phase.PlayCards)
            confirmUi.gameObject.SetActive(true);
        if (battleState.Phase != BattleV2Phase.PlayCards)
            confirmUi.gameObject.SetActive(false);
    }

    private void ConfirmEarly()
    {
        _confirmRequestedManually = true;
        Confirm();
    }
    
    public void Confirm()
    {
        var reasonWord = _confirmRequestedManually ? "manually" : "automatically";
        _confirmRequestedManually = false;
        if (confirmUi != null)
            confirmUi.gameObject.SetActive(false);
        playArea.Clear();
        if (!_alreadyConfirmedThisTurn)
            BattleLog.Write($"Turn {battleState.TurnNumber} {battleState.Phase} - Player turn ended {reasonWord}");
        _alreadyConfirmedThisTurn = true;
        Message.Publish(new PlayerTurnConfirmed());
    }

    public void Cancel()
    {
        _confirmRequestedManually = false;
    }
}
