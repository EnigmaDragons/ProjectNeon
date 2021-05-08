using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/CardPool")]
public class ShopCardPool : ScriptableObject
{
    [SerializeField] public List<CardType> allCards;
    [SerializeField] private List<ShopCardPool> subPools;

    //Editor Only Property
    public StringVariable[] archetypes;
    public Rarity[] includedRarities;
    
    public IEnumerable<CardType> All => subPools.SelectMany(s => s.All).Concat(allCards);

    public IEnumerable<CardType> Get(HashSet<string> includedArchetypes, params Rarity[] includedRarities)
    {
        var isSelectedPool = includedArchetypes.None() || archetypes.All(x => includedArchetypes.Contains(x.Value));
        if (isSelectedPool && (includedRarities == this.includedRarities || includedRarities.None()))
            return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities)).Concat(allCards);
        if (isSelectedPool && this.includedRarities.Any(r => includedRarities.Contains(r)))
            return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities))
                .Concat(allCards.Where(x => includedRarities.Contains(x.Rarity)));
        return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities));
    }
}
