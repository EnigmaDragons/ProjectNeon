using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLibrary : MonoBehaviour
{
    [SerializeField] private Library library;

    public Card[] select(Hero hero)
    {
        List<Card> cards = new List<Card>(0);
        library.UnlockedCards.ForEach(
            card =>
            {
                if (
                    card.LimitedToClass.IsPresent 
                )
                {
                    cards.Add(card);
                }
            }
        );
        return cards.ToArray();
    }

}
