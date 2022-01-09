using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleCardsView : MonoBehaviour
{
    [SerializeField] private CardPresenter[] cardPresenters;

    public int MaxCardsDisplayed => cardPresenters.Length;
    
    public void Show(IEnumerable<Card> cards)
    {
        cardPresenters.ForEach(card => card.gameObject.SetActive(false));
        var c = cards.ToArray();
        for(var i = 0; i < cardPresenters.Length && i < c.Length; i++)
            cardPresenters[i].Set(c[i]);
    }
}
