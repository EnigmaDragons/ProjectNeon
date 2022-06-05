using System.Linq;
using DG.Tweening;
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
    private Vector3 _confirmUiScale;

    private void Awake()
    {
        _confirmUiScale = confirmUi.gameObject.transform.localScale;
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
        if (WillAutomaticallyEndTurn())
        {
            DevLog.Write($"No playable cards. Requesting early turn Confirmation. Hand Size {battleState.PlayerCardZones.HandZone.Cards.Length}. Num Cycles {battleState.NumberOfRecyclesRemainingThisTurn}");
            Confirm();
        }
    }

    private bool WillAutomaticallyEndTurn() => battleState.Phase == BattleV2Phase.PlayCards &&
                                               (!battleState.PlayerCardZones.HandZone.HasCards
                                                || (battleState.NumberOfRecyclesRemainingThisTurn <= 0
                                                    && battleState.NumberOfCardPlaysRemainingThisTurn <= 0
                                                    && battleState.PlayerCardZones.PlayZone.IsEmpty
                                                    && battleState.PlayerCardZones.ResolutionZone.IsEmpty
                                                    && battleState.PlayerCardZones.HandZone.Cards.All(c =>
                                                        !c.IsAnyFormPlayableByHero(battleState.Party,
                                                            battleState.NumberOfCardPlaysRemainingThisTurn)
                                                        || !c.Owner.CanPlayCards())));
    
    private void UpdateState(BattleStateChanged msg)
    {
        if (battleState.Phase == BattleV2Phase.PlayCards && msg.Before.Phase != BattleV2Phase.PlayCards)
            _alreadyConfirmedThisTurn = false;
        if (battleState.Phase == BattleV2Phase.PlayCards && msg.State.NumberOfCardPlaysRemainingThisTurn == 0)
        {
            var g = confirmUi.gameObject;
            var shouldBeVisible = !WillAutomaticallyEndTurn();
            var activate = !g.activeSelf && shouldBeVisible;
            g.SetActive(shouldBeVisible);
            if (activate)
            {
                DOTween.Kill(g);
                g.transform.localScale = _confirmUiScale;
                g.transform.DOPunchScale(new Vector3(1.1f * _confirmUiScale.x, 1.1f * _confirmUiScale.y, 1.1f * _confirmUiScale.z), 0.5f, 1);
            }
        }

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
