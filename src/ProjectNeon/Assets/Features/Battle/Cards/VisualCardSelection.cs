using System;
using UnityEngine;

public sealed class VisualCardSelection : MonoBehaviour
{
    [SerializeField] private CardsVisualizer cards;
    [SerializeField] private GameEvent[] activateHighlightWhen;
    
    private IndexSelector<GameObject> _indexSelector;
    private bool _isDirty = false;
    private bool _shouldHighlight;
    
    private void OnEnable()
    {
        Message.Subscribe<TargetSelectionFinished>(_ => _isDirty = true, this);
        activateHighlightWhen.ForEach(e => e.Subscribe(() =>
        {
            _shouldHighlight = true;
            _isDirty = true;
        }, this));
        cards.SetOnShownCardsChanged(() => _isDirty = true);
        _isDirty = true;
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
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
        DisableHighlight();
        _indexSelector.MoveNext();
        EnableHighlight();
    }
    
    public void MovePrevious()
    {
        DisableHighlight();
        _indexSelector.MovePrevious();
        EnableHighlight();
    }

    public void Select()
    {
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
