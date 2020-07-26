using System.Linq;
using UnityEngine;

public sealed class SelectCardTargets : MonoBehaviour
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone selectedCardZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private GameEvent onTargetSelectionStarted;
    [SerializeField] private GameEvent onTargetSelectionFinished;
    [SerializeField] private GameObject uiView;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingState targetingState;

    [ReadOnly, SerializeField] private Card _selectedCard;
    private bool _isReadyForSelection;
    private Member _hero;
    private int _actionIndex;
    private int _numActions;
    private Target[] _actionTargets;
    
    private void Update()
    {
        if (_selectedCard == null) return;
        if (!_isReadyForSelection)
        {
            _isReadyForSelection = true;
            return;
        }

        // @todo #1:15min Replace this with OnConfirmOrCancel script
        
        if (Input.GetButtonDown("Submit"))
            OnTargetConfirmed();

        if (Input.GetButtonDown("Cancel"))
            OnCancelled();
    }

    private void OnEnable()
    {
        selectedCardZone.OnZoneCardsChanged.Subscribe(BeginSelection, this);
    }

    private void OnDisable()
    {
        selectedCardZone.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void BeginSelection()
    {
        if (selectedCardZone.Count < 1)
            return;
        
        battleState.SelectionStarted = true;
        onTargetSelectionStarted.Publish();
        _selectedCard = selectedCardZone.Cards[0];
        _isReadyForSelection = false;
        
        var cardClass = _selectedCard.LimitedToClass;
        if (!cardClass.IsPresent)
        {
            Debug.Log($"Card {_selectedCard.Name} is not playable by Heroes", _selectedCard);
            return;
        }

        cardPresenter.Set(_selectedCard, () => { });
        uiView.SetActive(true);

        _hero = battleState.Members.Values.FirstOrDefault(x => x.Class.Equals(cardClass.Value));
        if (_hero == null)
        {
            Debug.Log($"Could not find Party Member with Class {cardClass.Value}");
            return;
        }

        _actionIndex = 0;
        _numActions = _selectedCard.Actions.Length;
        _actionTargets = new Target[_numActions];
        if (_numActions == 0)
        {
            Debug.Log($"Card {_selectedCard.Name} has no Card Actions");
            OnTargetConfirmed();
            return;
        }

        PresentPossibleTargets();
    }

    private void PresentPossibleTargets()
    {
        var action = _selectedCard.Actions[_actionIndex];
        var possibleTargets = battleState.GetPossibleConsciousTargets(_hero, action.Group, action.Scope);
        targetingState.WithPossibleTargets(possibleTargets);
        if (possibleTargets.Length == 1)
            OnTargetConfirmed();
    }

    private void OnCancelled() => OnSelectionComplete(sourceCardZone);
    private void OnTargetConfirmed()
    {
        _actionTargets[_actionIndex] = targetingState.Current;
        targetingState.Clear();

        if (_actionIndex + 1 == _numActions)
        {
            cardResolutionZone.Add(new PlayedCard(_hero, _actionTargets, _selectedCard));
            OnSelectionComplete(destinationCardZone);
        }
        else
        {
            _actionIndex++;
            PresentPossibleTargets();
        }
    }

    private void OnSelectionComplete(CardPlayZone sendToZone)
    {
        sendToZone.PutOnBottom(selectedCardZone.DrawOneCard());
        _selectedCard = null;
        uiView.SetActive(false);
        onTargetSelectionFinished.Publish();
        battleState.SelectionStarted = false;
    }
}
