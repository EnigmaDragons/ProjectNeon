using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Enemies")]
public class AllEnemies : ScriptableObject
{
    [UnityEngine.UI.Extensions.ReadOnly] public Enemy[] Enemies; //Unity Collection Readonly

    public Dictionary<int, Enemy> GetMap() => Enemies.ToDictionary(x => x.id, x => x);
    public Maybe<Enemy> GetEnemyById(int id) => GetMap().ValueOrMaybe(id);
}
