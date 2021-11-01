using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Cards")]
public class AllCards : ScriptableObject
{
    private Dictionary<int, CardTypeData> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public CardType[] Cards; //Unity Collection Readonly

    public Dictionary<int, CardTypeData> GetMap() => _map ??= Cards.Where(c => c != null).ToDictionary(x => x.id, x => (CardTypeData)x);
    public Maybe<CardTypeData> GetCardById(int id) => GetMap().ValueOrMaybe(id);
}