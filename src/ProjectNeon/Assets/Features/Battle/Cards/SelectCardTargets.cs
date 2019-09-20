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

        // @todo #1:30min Get Possible Targets,as per Card Scope
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
