using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleCardsView : MonoBehaviour
{
    [SerializeField] private CardPresenter[] cardPresenters;

    public void Show(IEnumerable<Card> cards)
    {
        cardPresenters.ForEach(c => c.gameObject.SetActive(false));
        var c = cards.ToArray();
        for(var i = 0; i < cardPresenters.Length && i < c.Length; i++)
            cardPresenters[i].Set(c[i]);
    }
}
