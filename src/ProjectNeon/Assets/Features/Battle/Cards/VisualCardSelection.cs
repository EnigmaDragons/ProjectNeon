using UnityEngine;

public sealed class VisualCardSelection : MonoBehaviour
{
    [SerializeField] private CardsVisualizer cards;

    private IndexSelector<GameObject> _indexSelector;
    private bool _isDirty = false;

    private void OnEnable()
    {
        cards.SetOnShownCardsChanged(() => _isDirty = true);
    }

    private void Update()
    {
        if (!_isDirty)
            return;
        
        _isDirty = false;
        if (cards.ShownCards.Length < 1)
            return;
        
        _indexSelector = new IndexSelector<GameObject>(cards.ShownCards);
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(true);
    }

    public void MoveNext()
    {
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(false);
        _indexSelector.MoveNext().GetComponent<CardPresenter>().SetHighlight(true);
    }
    
    public void MovePrevious()
    {
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(false);
        _indexSelector.MovePrevious().GetComponent<CardPresenter>().SetHighlight(true);
    }

    // @todo #1:15min Don't allow selection from Hand when Selection Targets
    
    public void Select()
    {
        cards.SelectCard(_indexSelector.Index);
    }
}
