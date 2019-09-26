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

    [ReadOnly] [SerializeField] private Card _selectedCard;

    private void Update()
    {
        if (_selectedCard == null) return;

        if (Input.GetButton("Submit"))
            OnTargetConfirmed();

        if (Input.GetButton("Cancel"))
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

    // @todo #1:30min When Target Selection Starts, disable selecting a new card or cancelling a previous selection

    private void BeginSelection()
    {
        if (selectedCardZone.Count < 1)
            return;

        onTargetSelectionStarted.Publish();
        _selectedCard = selectedCardZone.Cards[0];
        cardPresenter.Set(_selectedCard, () => { });
        uiView.SetActive(true);

        var possibleTargets = battleState.GetPossiblePlayerTargets(_selectedCard.Actions[0].Group, _selectedCard.Actions[0].Scope);
        // @todo #207:30min Repeat target selection for all card actions. Currently we re just sorting possible targets for the first
        //  CardAction, but we need select target for all actions after the first one.

        // @todo #1:15min Needs to know who Self is, if this is a Hero card.
        
        // @todo #1:30min Create UI Indicator that can indicate possible selections
    }

    private void OnCancelled() => OnSelectionComplete(sourceCardZone);
    private void OnTargetConfirmed() => OnSelectionComplete(destinationCardZone);

    private void OnSelectionComplete(CardPlayZone sendToZone)
    {
        sendToZone.PutOnBottom(selectedCardZone.DrawOneCard());
        _selectedCard = null;
        uiView.SetActive(false);
        onTargetSelectionFinished.Publish();
    }
}
