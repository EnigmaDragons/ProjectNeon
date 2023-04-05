using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/PartyCardCollection")]
public sealed class PartyCardCollection : ScriptableObject
{
    [SerializeField] private List<CardType> allCards = new List<CardType>();

    private Dictionary<CardType, int> cardsWithCounts = new Dictionary<CardType, int>();

    public Dictionary<CardType, int> AllCards => cardsWithCounts;
    
    public PartyCardCollection Initialized(IEnumerable<CardType> cards)
    {
        allCards = new List<CardType>();
        allCards.AddRange(cards);
        cardsWithCounts = allCards
            .GroupBy(c => c.Name)
            .ToDictionary(c => c.First(), c => c.Count());
        return this;
    }

    public void Add(params CardType[] cards)
    {
        cards.ForEach(c =>
        {
            allCards.Add(c);
            if (!cardsWithCounts.ContainsKey(c))
            {
                cardsWithCounts[c] = 0;
            }

            cardsWithCounts[c] = cardsWithCounts[c] + 1;
        });
    }

    public void Remove(params CardType[] cards)
    {
        cards.ForEach(c =>
        {
            allCards.Remove(c);
            if (cardsWithCounts.ContainsKey(c))
                cardsWithCounts[c] = Math.Max(0, cardsWithCounts[c] - 1);
            if (cardsWithCounts[c] == 0)
                cardsWithCounts.Remove(c);
        });
    }
    
    public void Add(CardType card, int count)
    {
        allCards.Add(card);
        if (!cardsWithCounts.ContainsKey(card))
            cardsWithCounts[card] = 0;
        cardsWithCounts[card] = cardsWithCounts[card] + count;
    }

    public void EnsureHasAtLeast(CardType c, int numCopies)
    {
        var amount = cardsWithCounts.ContainsKey(c) ? cardsWithCounts[c] : 0;
        var numAdditionalNeeded = numCopies - amount;
        if (numAdditionalNeeded > 0)
            Enumerable.Range(0, numAdditionalNeeded).ForEach(_ => Add(c));
    }
    
    public CardType[] CardsForHero(BaseHero h)
        => AllCards
            .Where(cardWithCount => 
                cardWithCount.Key.Archetypes.All(archetype => h.Archetypes.Contains(archetype)) 
                && cardWithCount.Key.Id != h.BasicCard.Id)
            .OrderBy(c => c.Key.Archetypes.None() ? 999 : 0)
            .ThenByDescending(c => (int)c.Key.Rarity)
            .SelectMany(c => Enumerable.Range(0, c.Value).Select(i => c.Key))
            .ToArray();
}
