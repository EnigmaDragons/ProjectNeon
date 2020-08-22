using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/CardPool")]
public class ShopCardPool : ScriptableObject
{
    [SerializeField] private List<CardType> allCards;

    public IEnumerable<CardType> All => allCards;
}
