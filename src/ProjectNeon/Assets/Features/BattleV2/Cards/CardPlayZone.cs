using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/CardPlayZone")]
public class CardPlayZone : ScriptableObject
{
    [SerializeField] private IntReference maxCards = new IntReference(10);
    [SerializeField] private Card[] cards;
    [SerializeField] private GameEvent onZoneCardsChanged;

    public int Count => cards.Length;
    public int Max => maxCards.Value;
    public Card[] Cards => cards.ToArray();
    public GameEvent OnZoneCardsChanged => onZoneCardsChanged;
    public bool IsFull => Count >= Max;
    public bool IsEmpty => cards.Length == 0;
    public bool HasCards => cards.Length > 0;

    public void Init(IEnumerable<Card> newCards) => cards = newCards.ToArray();

    public void InitShuffled(IEnumerable<Card> cards)
    {
        Init(cards);
        Shuffle();
    }

    public Card DrawOneCard()
    {
        if (cards.Length == 0)
            throw new InvalidOperationException($"{name} has no cards");

        var newCard = cards[0];
        Mutate(c => c.Skip(1).ToArray());
        return newCard;
    }
    
    public Card Take(int index)
    {
        if (cards.Length < index)
            Log.Error($"Cannot Take Card {index} from {cards.Length} cards.");
        
        var card = cards[index];
        Mutate(c => c.Where((v, i) => i != index));
        return card;
    }
    
    public void Remove(Card card) => Mutate(c => c.Where(x => x.Id != card.Id));

    public void Clear() => Mutate(c => Array.Empty<Card>());
    public void PutOnTop(Card card) => Mutate(c => card.Concat(c));
    public void PutOnBottom(Card card) => Mutate(c => c.Concat(card));
    public void Shuffle() => Mutate(c => c.Shuffled());
    public void Set(params Card[] val) => Mutate(c => val);
    
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
