using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Cards")]
public class AllCards : ScriptableObject
{
    private Dictionary<int, CardType> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public CardType[] Cards; //Unity Collection Readonly

    public Dictionary<int, CardType> GetMap() => _map ??= Cards.Where(c => c != null).SafeToDictionaryWithLoggedErrors(x => x.id, x => x);
    public Maybe<CardType> GetCardById(int id) => GetMap().ValueOrMaybe(id);
}