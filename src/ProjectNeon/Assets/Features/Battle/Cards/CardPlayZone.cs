using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPlayZone : ScriptableObject
{
    [SerializeField] private IntReference maxCards = new IntReference(6);
    [SerializeField] private Card[] cards;
    [SerializeField] private GameEvent onZoneCardsChanged;

    public int Count => cards.Length;
    public Card[] Cards => cards.ToArray();
    public GameEvent OnZoneCardsChanged => onZoneCardsChanged;

    public void Init(IEnumerable<Card> newCards)
    {
        cards = newCards.ToArray();
    }
    
    public Card DrawOneCard()
    {
        var newCard = cards[0];
        Mutate(c => c.Skip(1).ToArray());
        return newCard;
    }

    public Card Take(int index)
    {
        var card = cards[index];
        Mutate(c => c.Where((v, i) => i != index));
        return card;
    }

    public void Clear() => Mutate(c => Array.Empty<Card>());
    public void PutOnTop(Card card) => Mutate(c => card.Concat(c));
    public void PutOnBottom(Card card) => Mutate(c => c.Concat(card));
    public void Shuffle() => Mutate(c => c.Shuffled());

    public void Mutate(Func<Card[], IEnumerable<Card>> update)
    {
        if (cards == null)
            cards = new Card[0];
        var newVal = update(cards).ToArray();
        if (newVal.Length > maxCards.Value)
            throw new InvalidOperationException($"{name} can hold a Maximum of {maxCards.Value} Cards");
        cards = newVal;
        onZoneCardsChanged.Publish();
    }
}
