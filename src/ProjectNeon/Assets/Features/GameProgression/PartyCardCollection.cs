using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/PartyCardCollection")]
public sealed class PartyCardCollection : ScriptableObject
{
    [SerializeField] private List<CardTypeData> allCards = new List<CardTypeData>();

    private Dictionary<CardTypeData, int> cardsWithCounts = new Dictionary<CardTypeData, int>();

    public Dictionary<CardTypeData, int> AllCards => cardsWithCounts;
    
    public PartyCardCollection Initialized(IEnumerable<CardTypeData> cards)
    {
        allCards = new List<CardTypeData>();
        allCards.AddRange(cards);
        cardsWithCounts = allCards
            .GroupBy(c => c.Name)
            .ToDictionary(c => c.First(), c => c.Count());
        return this;
    }

    public void Add(params CardTypeData[] cards)
    {
        cards.ForEach(c =>
        {
            allCards.Add(c);
            if (!cardsWithCounts.ContainsKey(c))
                cardsWithCounts[c] = 0;
            cardsWithCounts[c] = cardsWithCounts[c] + 1;
        });
    }

    public void Remove(params CardTypeData[] cards)
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
    
    public void Add(CardTypeData card, int count)
    {
        allCards.Add(card);
        if (!cardsWithCounts.ContainsKey(card))
            cardsWithCounts[card] = 0;
        cardsWithCounts[card] = cardsWithCounts[card] + count;
    }

    public void EnsureHasAtLeast(CardTypeData c, int numCopies)
    {
        var amount = cardsWithCounts.ContainsKey(c) ? cardsWithCounts[c] : 0;
        var numAdditionalNeeded = numCopies - amount;
        if (numAdditionalNeeded > 0)
            Enumerable.Range(0, numAdditionalNeeded).ForEach(_ => Add(c));
    }
}
