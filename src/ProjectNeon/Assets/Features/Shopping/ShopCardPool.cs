using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/CardPool")]
public class ShopCardPool : ScriptableObject
{
    //public so editor script can access
    public StringVariable[] archetypes;
    public Rarity[] includedRarities;
    [SerializeField] private List<ShopCardPool> subPools;
    [UnityEngine.UI.Extensions.ReadOnly] public List<CardType> allCards;

    private string[] _archetypes;
    private string[] Archetypes => _archetypes ??= archetypes.Select(x => x.Value).ToArray();

    public IEnumerable<CardType> All => subPools.SelectMany(s => s.All).Concat(allCards);

    public IEnumerable<CardType> Get(HashSet<string> includedArchetypes, params Rarity[] includedRarities)
    {
        var isSelectedPool = includedArchetypes.None() || Archetypes.All(includedArchetypes.Contains);
        if (isSelectedPool && (includedRarities == this.includedRarities || includedRarities.None()))
            return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities)).Concat(allCards);
        if (isSelectedPool && this.includedRarities.Any(includedRarities.Contains))
            return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities))
                .Concat(allCards.Where(x => includedRarities.Contains(x.Rarity)));
        return subPools.SelectMany(x => x.Get(includedArchetypes, includedRarities));
    }
    
    public ShopCardPool Initialized(string[] archetypes, Rarity[] rarities, IEnumerable<ShopCardPool> subPools, IEnumerable<CardType> cards)
    {
        _archetypes = archetypes;
        this.includedRarities = rarities;
        this.subPools = subPools.ToList();
        allCards = cards.ToList();
        return this;
    }
}
