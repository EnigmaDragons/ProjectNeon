using System;
using System.Linq;
using UnityEngine;

public sealed class VisualCardSelectionV2 : MonoBehaviour, IDirectionControllable, IConfirmCancellable
{
    [SerializeField] private HandVisualizer cards;
    [SerializeField] private BattleState state;
    
    private IndexSelector<CardPresenter> _indexSelector;
    private bool _isDirty = false;
    private bool _shouldHighlight;
    private bool _isConfirmingTurn = false;
    private int _lastIndex;
    
    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(_ => _isDirty = true, this);
        Message.Subscribe<TurnStarted>(_ => Activate(), this);
        Message.Subscribe<PlayerTurnConfirmationAborted>(_ => SetIsConfirming(false), this);
        Message.Subscribe<PlayerTurnConfirmationStarted>(_ => SetIsConfirming(true), this);
        Message.Subscribe<ToggleUseCardAsBasic>(_ => ToggleAsBasic(), this);
        Message.Subscribe<RecycleCard>(_ => Recycle(), this);
        Message.Subscribe<CardHoverEnter>(c => MoveTo(c.Card), this);
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
        cards.SetCardPlayingAllowed(true);
        cards.UpdateVisibleCards();
    }
    
    private void Update()
    {
        if (!_isDirty)
            return;
        
        _isDirty = false;
        if (cards.ShownCards.Length < 1)
            return;

        UpdateUi();
    }

    private void UpdateUi()
    {
        if (_isConfirmingTurn)
            _shouldHighlight = false;

        var activeCards = cards.ShownCards;
        _indexSelector = new IndexSelector<CardPresenter>(activeCards, Math.Min(activeCards.Length - 1, _lastIndex));
        if (activeCards.Any(c => c.HasCard))
            while (!_indexSelector.Current.HasCard)
                _indexSelector.MovePrevious();
        _lastIndex = _indexSelector.Index;
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
        _lastIndex = _indexSelector.Index;
        EnableHighlight();
    }
    
    public void MovePrevious()
    {
        DisableHighlight();
        _indexSelector.MovePrevious();
        while(!_indexSelector.Current.HasCard)
            _indexSelector.MovePrevious();
        _lastIndex = _indexSelector.Index;
        EnableHighlight();
    }

    public void MoveTo(CardPresenter c)
    {
        DisableHighlight();
        while(_indexSelector.Current != c)
            _indexSelector.MoveNext();
        _lastIndex = _indexSelector.Index;
        EnableHighlight();
    }

    public void LostFocus()
    {
        Debug.Log("Hand Lost Focus");
        _shouldHighlight = false;
        DisableHighlight();
    }
    
    public void Cancel() {}
    public void Confirm() {}
    public void Select()
    {
        DisableHighlight();
        _shouldHighlight = false;
        Message.Publish(new PlayerCardSelected());
        cards.SelectCard(_indexSelector.Index);
        UpdateUi();
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
        cards.ShownCards.ForEach(c => c.SetHandHighlight(false));
        _indexSelector.Current.SetHandHighlight(true);
    }
    
    private void DisableHighlight()
    {
        _indexSelector.Current.SetHandHighlight(false);
    }
}
