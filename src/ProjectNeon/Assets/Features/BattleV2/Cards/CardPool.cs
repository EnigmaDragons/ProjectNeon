using System.Linq;
using UnityEngine;

public sealed class CardPool
{
    private readonly CardPresenter[] _cards;

    public CardPresenter this[int index] => _cards[index];
    public CardPresenter[] ShownCards => _cards.ToArray();

    public CardPool(int size, MonoBehaviour owner, CardPresenter prototype, Vector3 cardRotation)
    {
        _cards = new CardPresenter[size];
        for (var i = 0; i < size; i++)
        {
            _cards[i] = Object.Instantiate(prototype, owner.transform);
            _cards[i].Clear();
            _cards[i].transform.rotation = Quaternion.Euler(cardRotation);
        }
    }
    
    public (int, CardPresenter) GetCardPresenter(int startAtIndex, Card c)
    {
        CardPresenter emptyCard = null;
        var emptyCardIndex = -1;
        
        for (var i = startAtIndex; i < _cards.Length; i++)
        {
            var cp = _cards[i];
            
            // Find First Unused Card Presenter
            if (emptyCard == null && !cp.HasCard)
            {
                emptyCard = cp;
                emptyCardIndex = i;
            }

            // Return Matching Card
            if (cp.Contains(c))
                return (i, cp);
        }

        return (emptyCardIndex, emptyCard);
    }

    public void SwapItems(int first, int second) => _cards.SwapItems(first, second);
}
