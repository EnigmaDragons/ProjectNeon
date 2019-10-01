using UnityEngine;

public sealed class VisualCardSelection : MonoBehaviour
{
    [SerializeField] private CardsVisualizer cards;

    private bool _isDirty = true;
    private int _currentIndex = 0;

    private void Update()
    {
        if (!_isDirty)
            return;
        
        if (cards.ShownCards.Length > 0)
        {
            cards.ShownCards[_currentIndex].GetComponent<CardPresenter>().SetHighlight(true);
            _isDirty = false;
        }
    }
}
