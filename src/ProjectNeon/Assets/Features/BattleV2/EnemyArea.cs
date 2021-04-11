using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/EnemyArea")]
public class EnemyArea : ScriptableObject
{
    [SerializeField] private List<EnemyInstance> enemies;
    [SerializeField] private List<Transform> uiPositions;

    public List<EnemyInstance> Enemies => enemies;
    public List<Transform> EnemyUiPositions => uiPositions;

    public EnemyArea Initialized(IEnumerable<EnemyInstance> newEnemies)
    {
        enemies = newEnemies.ToList();
        return this;
    }

    public EnemyArea WithUiTransforms(IEnumerable<Transform> positions)
    {
        uiPositions = positions.ToList();
        return this;
    }

    public void Add(EnemyInstance e, Transform uiPosition)
    {
        enemies.Add(e);
        uiPositions.Add(uiPosition);
    }
    
    public void Remove(EnemyInstance enemy)
    {
        var index = enemies.IndexOf(enemy);
        enemies.RemoveAt(index);
        uiPositions.RemoveAt(index);
    }
    
    public void Clear() => enemies = new List<EnemyInstance>();
}
