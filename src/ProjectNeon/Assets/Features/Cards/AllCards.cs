using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Cards")]
public class AllCards : ScriptableObject
{
    private Dictionary<int, CardTypeData> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public CardType[] Cards;

    public Dictionary<int, CardTypeData> GetMap() => _map ??= Cards.ToDictionary(x => x.Id, x => (CardTypeData)x);
}