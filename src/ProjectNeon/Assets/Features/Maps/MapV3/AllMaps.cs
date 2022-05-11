using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Maps")]
public class AllMaps : ScriptableObject
{
    private Dictionary<int, GameMap3> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public GameMap3[] Maps; //Unity Collection Readonly

    public Dictionary<int, GameMap3> GetMap() => _map ??= Maps.ToDictionary(x => x.id, x => x);
    public Maybe<GameMap3> GetMapById(int id) => GetMap().ValueOrMaybe(id);
}