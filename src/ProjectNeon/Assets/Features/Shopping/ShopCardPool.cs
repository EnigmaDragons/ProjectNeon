using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/CardPool")]
public class ShopCardPool : ScriptableObject
{
    //public so editor script can access
    public StringVariable[] archetypes;
    public Rarity[] includedRarities = {Rarity.Starter, Rarity.Basic, Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic};
    [SerializeField] private List<ShopCardPool> subPools;
    [UnityEngine.UI.Extensions.ReadOnly] public List<CardType> allCards; //Unity Collection Readonly
    
    public IEnumerable<CardTypeData> All => subPools.SelectMany(s => s.All).Concat(allCards).Distinct();

    public IEnumerable<CardTypeData> Get(HashSet<string> archetypesToGet, HashSet<int> unobtainableCards, params Rarity[] raritiesToGet)
    {
        var isSelectedPool = archetypesToGet.None() || Archetypes().All(archetypesToGet.Contains);
        if (isSelectedPool && (includedRarities.All(raritiesToGet.Contains) || raritiesToGet.None()))
            return subPools.SelectMany(subPool => subPool.Get(archetypesToGet, unobtainableCards, raritiesToGet))
                .Concat(allCards.Where(card => !unobtainableCards.Contains(card.Id)));
        if (isSelectedPool && includedRarities.Any(raritiesToGet.Contains))
            return subPools.SelectMany(subPool => subPool.Get(archetypesToGet, unobtainableCards, raritiesToGet))
                .Concat(allCards
                    .Where(card => raritiesToGet.Contains(card.Rarity) && !unobtainableCards.Contains(card.Id)));
        return subPools.SelectMany(subPool => subPool.Get(archetypesToGet, unobtainableCards, raritiesToGet));
    }

    public ShopCardPool Initialized(string[] archetypesToSet, Rarity[] rarities, IEnumerable<ShopCardPool> subPoolsToSet, IEnumerable<CardType> cards)
    {
        _archetypes = archetypesToSet;
        includedRarities = rarities;
        subPools = subPoolsToSet.ToList();
        allCards = cards.ToList();
        return this;
    }

    private string[] _archetypes;

    private string[] Archetypes()
    {
        //Thanks Unity Serialization
        if (_archetypes == null || (_archetypes.Length == 0 && archetypes != null && archetypes.Length > 0))
            _archetypes = archetypes.Select(x => x.Value).ToArray();
        return _archetypes;
    } 
}
