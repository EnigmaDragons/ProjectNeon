using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/CardPool")]
public class ShopCardPool : ScriptableObject
{
    [SerializeField] private List<CardType> allCards;
    [SerializeField] private List<ShopCardPool> subPools;

    public IEnumerable<CardType> All => subPools.SelectMany(s => s.All).Concat(allCards);
    public IEnumerable<CardType> AllExceptStarters => subPools
        .SelectMany(s => s.AllExceptStarters)
        .Concat(allCards.Where(x => x.Rarity != Rarity.Starter));
}
