using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyArea : ScriptableObject
{
    [SerializeField] private Enemy[] enemies;

    public Enemy[] Enemies => enemies.ToArray();

    public EnemyArea Initialized(IEnumerable<Enemy> newEnemies)
    {
        enemies = newEnemies.ToArray();
        return this;
    }
}
