using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CardPlayZone : ScriptableObject
{
    public Card[] Cards;
    public GameEvent OnZoneCardsChanged;

    public Card DrawOneCard()
    {
        var newCard = Cards[0];
        Cards = Cards.Skip(1).ToArray();
        OnZoneCardsChanged.Publish();
        return newCard;
    }

   public void Clear()
   {
        Cards = Array.Empty<Card>();
        OnZoneCardsChanged.Publish();
   }

    public Card Take(int index)
    {
        var card = Cards[index];
        Cards = Cards.Where((v, i) => i != index).ToArray();
        OnZoneCardsChanged.Publish();
        return card;
    }

    public void PutOnTop(Card card)
    {
        Cards = card.Concat(Cards).ToArray();
        OnZoneCardsChanged.Publish();
    }

    public void PutOnBottom(Card card)
    {
        Cards = Cards.Concat(card).ToArray();
        OnZoneCardsChanged.Publish();
    }

    public void Shuffle()
    {
        Cards = Cards.OrderBy((a) => (int)Math.Round(UnityEngine.Random.value, 0)).ToArray();
        OnZoneCardsChanged.Publish();
    }
}
