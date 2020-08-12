using UnityEngine;

public sealed class VisualCardSelectionV2 : MonoBehaviour, IDirectionControllable, IConfirmCancellable
{
    [SerializeField] private HandVisualizer cards;
    [SerializeField] private BattleState state;
    
    private IndexSelector<CardPresenter> _indexSelector;
    private bool _isDirty = false;
    private bool _shouldHighlight;
    private bool _isConfirmingTurn = false;
    
    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(_ => _isDirty = true, this);
        Message.Subscribe<TurnStarted>(_ => Activate(), this);
        Message.Subscribe<TargetSelectionFinished>(_ => Activate(), this);
        Message.Subscribe<PlayerTurnConfirmationAborted>(_ => SetIsConfirming(false), this);
        Message.Subscribe<TargetSelectionBegun>(_ => Deactivate(), this);
        Message.Subscribe<PlayerTurnConfirmationStarted>(_ => SetIsConfirming(true), this);
        Message.Subscribe<ToggleUseCardAsBasic>(_ => ToggleAsBasic(), this);
        Message.Subscribe<RecycleCard>(_ => Recycle(), this);
        cards.SetOnShownCardsChanged(() => _isDirty = true);
        _isDirty = true;
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void ToggleAsBasic()
    {
        _indexSelector.Current.ToggleAsBasic();
        cards.ReProcess();
    }
    
    private void SetIsConfirming(bool isConfirming)
    {
        _isConfirmingTurn = isConfirming;
        _isDirty = true;
        cards.SetFocus(!_isConfirmingTurn);
        if (!isConfirming)
            _shouldHighlight = true;
    }
    
    private void Activate()
    {
        _isDirty = true;
        _shouldHighlight = true;
    }

    private void Deactivate()
    {
        _isDirty = true;
        _shouldHighlight = false;
    }
    
    private void Update()
    {
        if (!_isDirty)
            return;
        
        _isDirty = false;
        if (cards.ShownCards.Length < 1)
            return;

        if (_isConfirmingTurn)
            _shouldHighlight = false;

        _indexSelector = new IndexSelector<CardPresenter>(cards.ShownCards);
        if (_shouldHighlight)
            EnableHighlight();
        else
            DisableHighlight();
    }

    public void MoveNext()
    {
        DisableHighlight();
        _indexSelector.MoveNext();
        while(!_indexSelector.Current.HasCard)
            _indexSelector.MoveNext();
        EnableHighlight();
    }
    
    public void MovePrevious()
    {
        DisableHighlight();
        _indexSelector.MovePrevious();
        while(!_indexSelector.Current.HasCard)
            _indexSelector.MovePrevious();
        EnableHighlight();
    }

    public void LostFocus()
    {
        _shouldHighlight = false;
        DisableHighlight();
    }
    
    public void Cancel() {}
    public void Confirm() => Select();
    public void Select()
    {
        DisableHighlight();
        _shouldHighlight = false;
        Message.Publish(new PlayerCardSelected());
        _indexSelector.Current.SetCardHandControlsVisible(false);
        cards.SelectCard(_indexSelector.Index);
    }

    private void Recycle()
    {
        if (state.NumberOfRecyclesRemainingThisTurn < 1)
            return;
        
        cards.RecycleCard(_indexSelector.Index);
        _isDirty = true;
    }

    private void EnableHighlight()
    {
        cards.ShownCards.ForEach(c => c.SetHighlight(false));
        _indexSelector.Current.SetHighlight(true);
        _indexSelector.Current.SetCardHandControlsVisible(true);
    }
    
    private void DisableHighlight()
    {
        _indexSelector.Current.SetHighlight(false);
        _indexSelector.Current.SetCardHandControlsVisible(false);
    }
}
