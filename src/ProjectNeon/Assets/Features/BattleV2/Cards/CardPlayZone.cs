using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/CardPlayZone")]
public class CardPlayZone : ScriptableObject
{
    [SerializeField] private IntReference maxCards = new IntReference(10);
    [SerializeField] private Card[] cards;
    [SerializeField] private GameEvent onZoneCardsChanged;

    private DeterministicRng _rng;

    private DeterministicRng Rng
    {
        get
        {
            if (_rng == null)
                _rng = new DeterministicRng(Rng.Seed);
            return _rng;
        }
    }

    public int Count => cards.Length;
    public int Max => maxCards.Value;
    public Card[] Cards => cards.ToArray();
    public GameEvent OnZoneCardsChanged => onZoneCardsChanged;
    public bool IsFull => Count >= Max;
    public bool IsEmpty => cards.Length == 0;
    public bool HasCards => cards.Length > 0;

    public void Init(IEnumerable<Card> newCards)
    {
        cards = newCards.ToArray();
    }

    public void InitShuffled(IEnumerable<Card> newCards)
    {
        Init(newCards);
        _rng = new DeterministicRng(Rng.Seed);
        Shuffle();
    }

    public void InitShuffled(IEnumerable<Card> newCards, DeterministicRng rng)
    {
        Init(newCards);
        _rng = rng;
        Shuffle(rng);
    }
    
    public Card DrawOneCard()
    {
        if (cards.Length == 0)
            throw new InvalidOperationException($"{name} has no cards");

        var newCard = cards[0];
        Mutate(c => c.Skip(1).ToArray());
        return newCard;
    }

    public Card DrawOneCard(Func<Card, bool> cardCondition)
    {
        if (!cards.Any(cardCondition))
            throw new InvalidOperationException($"{name} has no cards that meet the condition");

        var newCard = cards.First(cardCondition);
        Mutate(c => c.Where(x => x != newCard).ToArray());
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
    
    public void Remove(Card card) => Mutate(c => c.Where(x => x.CardId != card.CardId));
    public void Replace(Card replaced, Card replacer) => Mutate(c => c.Select(x => x.CardId == replaced.CardId ? replacer : x));
    public void Clear() => Mutate(c => Array.Empty<Card>());
    public void PutOnTop(Card card) => Mutate(c => card.Concat(c));
    public void PutOnBottom(Card card) => Mutate(c => c.Concat(card));
    public void Shuffle() => Shuffle(Rng);
    private void Shuffle(DeterministicRng rng) => Mutate(c => rng.Shuffled(c.OrderBy(x => x.Name).ToArray()));
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
    
    public static CardPlayZone InMemory
    {
        get
        {
            var zone = (CardPlayZone)FormatterServices.GetUninitializedObject(typeof(CardPlayZone));
            zone.maxCards = new IntReference(99);
            zone.onZoneCardsChanged = GameEvent.InMemory;
            return zone;
        }
    }
}
