using TMPro;
using UnityEngine;

public class ClassCardsPages : MonoBehaviour
{
    [SerializeField] private GameObject cardPrototype;
    [SerializeField] private Library library;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI pageNumber;

    private CardPage[] _cardPages = new CardPage[0];
    private IndexSelector<Card[][]> _pageSelector;
    
    void OnEnable()
    {
        state.OnCurrentDeckChanged.Subscribe(Init, this);
    }

    void OnDisable()
    {
        state.OnCurrentDeckChanged.Unsubscribe(this);
    }

    // @todo #1:30min Selected relevant cards from library and paginate them
    void Init()
    {
    }

    void DisplayCurrentPage()
    {
        pageNumber.text = (_pageSelector.Index + 1).ToString();
    }
    
    // @todo #141: 30min Change page by swipe
    public void NextPage()
    {
        if (_cardPages.Length == 0)
            return;

        _pageSelector.MoveNext();
        DisplayCurrentPage();
        
    }
    
    public void PreviousPage()
    {
        if (_cardPages.Length == 0)
            return;
        
        _pageSelector.MovePrevious();
        DisplayCurrentPage();
    }
}
