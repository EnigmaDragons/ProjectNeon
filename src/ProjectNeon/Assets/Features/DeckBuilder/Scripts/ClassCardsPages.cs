using System;
using TMPro;
using UnityEngine;

public class ClassCardsPages : MonoBehaviour
{
    [SerializeField] private GameObject cardPrototype;
    [SerializeField] private Library library;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI pageNumber;

    private CardPage[] _cardPages;
    private IndexSelector<Card[][]> _pageSelector;
    
    void OnEnable()
    {
        state.OnCurrentDeckChanged.Subscribe(Init, this);
    }

    void OnDisable()
    {
        state.OnCurrentDeckChanged.Unsubscribe(this);
    }

    void Init()
    {
        int pages = Convert.ToInt16(Math.Truncate(Convert.ToDecimal(state.Current().Cards.Count / 8)));
        _cardPages = new CardPage[pages];

        for(int page = 0; page < pages; page++)
        {
            for (int card = 0; card < 8; card++)
            {
                int cardNumber = page * 8 + card;
                if (cardNumber < state.Current().Cards.Count)
                    _cardPages[page].Cards[card] = state.Current().Cards[page * 8 + card];
                else
                    break;
            }
        }
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
