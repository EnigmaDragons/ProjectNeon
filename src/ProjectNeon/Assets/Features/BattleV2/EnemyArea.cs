using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/EnemyArea")]
public class EnemyArea : ScriptableObject
{
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Transform[] uiPositions;

    public Enemy[] Enemies => enemies;
    public Transform[] EnemyUiPositions => uiPositions;

    public EnemyArea Initialized(IEnumerable<Enemy> newEnemies)
    {
        enemies = newEnemies.ToArray();
        return this;
    }

    public EnemyArea WithUiTransforms(IEnumerable<Transform> positions)
    {
        uiPositions = positions.ToArray();
        return this;
    }

    public void Clear() => enemies = new Enemy[0];
}
