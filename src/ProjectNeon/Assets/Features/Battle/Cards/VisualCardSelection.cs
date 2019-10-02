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

        // @todo #1:15min Don't show highlight on Card unless player can select a card from Hand. 
        
        _indexSelector = new IndexSelector<GameObject>(cards.ShownCards);
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(true);
    }

    public void MoveNext()
    {
        DisableCurrentCardHighlight();
        _indexSelector.MoveNext().GetComponent<CardPresenter>().SetHighlight(true);
    }
    
    public void MovePrevious()
    {
        DisableCurrentCardHighlight();
        _indexSelector.MovePrevious().GetComponent<CardPresenter>().SetHighlight(true);
    }

    public void Select()
    {
        DisableCurrentCardHighlight();
        cards.SelectCard(_indexSelector.Index);
    }

    private void DisableCurrentCardHighlight()
    {
        _indexSelector.Current.GetComponent<CardPresenter>().SetHighlight(false);
    }
}
