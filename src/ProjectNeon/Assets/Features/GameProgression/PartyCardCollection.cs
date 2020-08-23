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
        allCards.Clear();
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
                cardsWithCounts[c] = 0;
            cardsWithCounts[c] = cardsWithCounts[c] + 1;
        });
    }
}
