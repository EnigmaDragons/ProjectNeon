using UnityEngine;

public sealed class VisualCardSelectionV2 : MonoBehaviour, IDirectionControllable, IConfirmCancellable
{
    [SerializeField] private CardsVisualizer cards;
    
    private IndexSelector<GameObject> _indexSelector;
    private bool _isDirty = false;
    private bool _shouldHighlight;

    private bool _allowInput = false;
    
    private void OnEnable()
    {
        Message.Subscribe<TurnStarted>(_ => Activate(), this);
        Message.Subscribe<TargetSelectionFinished>(_ => Activate(), this);
        Message.Subscribe<TargetSelectionBegun>(_ => Deactivate(), this);
        cards.SetOnShownCardsChanged(() => _isDirty = true);
        _isDirty = true;
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void Activate()
    {
        _isDirty = true;
        _shouldHighlight = true;
        _allowInput = true;
    }

    private void Deactivate()
    {
        _allowInput = false;
    }
    
    private void Update()
    {
        if (!_isDirty)
            return;
        
        _isDirty = false;
        if (cards.ShownCards.Length < 1)
            return;

        _indexSelector = new IndexSelector<GameObject>(cards.ShownCards);
        if (_shouldHighlight)
            EnableHighlight();
    }

    public void MoveNext()
    {
        if (!_allowInput) return;
        
        DisableHighlight();
        _indexSelector.MoveNext();
        EnableHighlight();
    }
    
    public void MovePrevious()
    {
        if (!_allowInput) return;
        
        DisableHighlight();
        _indexSelector.MovePrevious();
        EnableHighlight();
    }

    public void Cancel() {}
    public void Confirm() => Select();
    public void Select()
    {
        if (!_allowInput) return;
        
        DisableHighlight();
        _shouldHighlight = false;
        cards.SelectCard(_indexSelector.Index);
    }

    private void EnableHighlight()
    {
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(true);
    }
    
    private void DisableHighlight()
    {
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(false);
    }
}