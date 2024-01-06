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
    [SerializeField] private Button endEarlyButton;
    [SerializeField] private Button endTurnInvisible;

    private bool _isConfirming = false;
    private bool _confirmRequestedManually;
    private bool _alreadyConfirmedThisTurn = false;
    private Vector3 _confirmUiScale;
    private BattleResolutions _battleResolutions;

    private void Awake()
    {
        _confirmUiScale = confirmUi.gameObject.transform.localScale;
        confirmUi.gameObject.SetActive(false);
        confirmUi.onClick.AddListener(ConfirmEarly);
        endEarlyButton.onClick.AddListener(ConfirmEarly);
        endTurnInvisible.onClick.AddListener(ConfirmEarly);
    }

    public void Init(BattleResolutions b) => _battleResolutions = b;
    
    private void OnEnable()
    {
        Message.Subscribe<BattleStateChanged>(msg => UpdateState(msg), this);
        Message.Subscribe<BeginPlayerTurnConfirmation>(_ => OnConfirmationRequested(), this);
        Message.Subscribe<CheckForAutomaticTurnEnd>(msg => CheckForAutomaticTurnEnd(), this);
        Message.Subscribe<CardAndEffectsResolutionFinished>(msg => CheckForAutomaticTurnEnd(), this);
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
        if (_battleResolutions != null && !_battleResolutions.IsDoneResolving)
            return;
        
        if (WillAutomaticallyEndTurn())
        {
            #if UNITY_EDITOR
            DevLog.Write($"No playable cards. Requesting early turn Confirmation. " +
                         $"Hand Size {battleState.PlayerCardZones.HandZone.Cards.Length}. Num Cycles {battleState.NumberOfRecyclesRemainingThisTurn}");
            #endif
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
        // Play Phase Started
        if (battleState.Phase == BattleV2Phase.PlayCards && msg.Before.Phase != BattleV2Phase.PlayCards)
        {
            _alreadyConfirmedThisTurn = false;
            if (!msg.State.IsTutorialCombat)
                endEarlyButton.gameObject.SetActive(true);
        }
        // Nearly Done With Play Phase
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
            endEarlyButton.gameObject.SetActive(false);
        }
        //in case you gained card plays
        if (battleState.Phase == BattleV2Phase.PlayCards && msg.State.NumberOfCardPlaysRemainingThisTurn != 0)
        {
            confirmUi.gameObject.SetActive(false);
        }
        // Out of Play Phase
        if (battleState.Phase != BattleV2Phase.PlayCards)
        {
            confirmUi.gameObject.SetActive(false);
            endEarlyButton.gameObject.SetActive(false);
        }
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
        confirmUi.gameObject.SetActive(false);
        endEarlyButton.gameObject.SetActive(false);
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
