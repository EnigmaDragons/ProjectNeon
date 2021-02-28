using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/EnemyArea")]
public class EnemyArea : ScriptableObject
{
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private List<Transform> uiPositions;

    public List<Enemy> Enemies => enemies;
    public List<Transform> EnemyUiPositions => uiPositions;

    public EnemyArea Initialized(IEnumerable<Enemy> newEnemies)
    {
        enemies = newEnemies.ToList();
        return this;
    }

    public EnemyArea WithUiTransforms(IEnumerable<Transform> positions)
    {
        uiPositions = positions.ToList();
        return this;
    }

    public void Add(Enemy e, Transform uiPosition)
    {
        enemies.Add(e);
        uiPositions.Add(uiPosition);
    }
    
    public void Remove(Enemy enemy)
    {
        var index = enemies.IndexOf(enemy);
        enemies.RemoveAt(index);
        uiPositions.RemoveAt(index);
    }
    
    public void Clear() => enemies = new List<Enemy>();
}
