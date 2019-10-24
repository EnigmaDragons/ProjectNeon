using System.Linq;
using UnityEngine;

class SelectCardTargets : MonoBehaviour
{
    [SerializeField] private CardPlayZone selectedCardZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private GameEvent onTargetSelectionStarted;
    [SerializeField] private GameEvent onTargetSelectionFinished;
    [SerializeField] private GameObject uiView;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingState targetingState;

    [ReadOnly, SerializeField, DTValidator.Optional] private Card _selectedCard;
    private bool _isReadyForSelection;

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

        var hero = battleState.Members.Values.FirstOrDefault(x => x.Class.Equals(cardClass.Value));
        if (hero == null)
        {
            Debug.Log($"Could not find Party Member with Class {cardClass.Value}");
            return;
        }

        var actions = _selectedCard.Actions;
        if (actions.Length == 0)
        {
            Debug.Log($"Card {_selectedCard.Name} has no Card Actions");
            OnTargetConfirmed();
            return;
        }

        var possibleTargets = battleState.GetPossibleTargets(hero, _selectedCard.Actions[0].Group, _selectedCard.Actions[0].Scope);
        // @todo #207:30min Repeat target selection for all card actions. Currently we re just sorting possible targets for the first
        //  CardAction, but we need select target for all actions after the first one.

        targetingState.WithPossibleTargets(possibleTargets);
    }

    private void OnCancelled() => OnSelectionComplete(sourceCardZone);
    private void OnTargetConfirmed() => OnSelectionComplete(destinationCardZone);

    private void OnSelectionComplete(CardPlayZone sendToZone)
    {
        // @todo #1:15 Put in Card Resolution Zone
        sendToZone.PutOnBottom(selectedCardZone.DrawOneCard());
        _selectedCard = null;
        uiView.SetActive(false);
        onTargetSelectionFinished.Publish();
        battleState.SelectionStarted = false;
    }
}
