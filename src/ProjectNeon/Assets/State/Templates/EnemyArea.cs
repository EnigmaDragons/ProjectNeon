using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyArea : ScriptableObject
{
    [SerializeField] private Enemy[] enemies;

    public Enemy[] Enemies => enemies.ToArray();
    
    public void Init(IEnumerable<Enemy> newEnemies) => enemies = newEnemies.ToArray();
}
