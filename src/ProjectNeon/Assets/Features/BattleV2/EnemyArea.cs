using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/EnemyArea")]
public class EnemyArea : ScriptableObject
{
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private List<Transform> uiPositions;
    [SerializeField] private List<Vector3> centerPoints;

    public List<Enemy> Enemies => enemies;
    public List<Transform> EnemyUiPositions => uiPositions;
    public List<Vector3> CenterPoints => centerPoints;

    public EnemyArea Initialized(IEnumerable<Enemy> newEnemies)
    {
        enemies = newEnemies.ToList();
        return this;
    }

    public EnemyArea WithUiTransforms(IEnumerable<Transform> positions, IEnumerable<Vector3> centers)
    {
        uiPositions = positions.ToList();
        centerPoints = centers.ToList();
        return this;
    }

    public void Add(Enemy e, Transform uiPosition)
    {
        enemies.Add(e);
        uiPositions.Add(uiPosition);
    }
    
    public void Clear() => enemies = new List<Enemy>();
}
