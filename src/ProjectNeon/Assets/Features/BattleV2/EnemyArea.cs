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

    public EnemyArea WithUiTransforms(IEnumerable<Transform> positions)
    {
        uiPositions = positions.ToList();
        return this;
    }

    public void WithCenterPoints()
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            var centerPoint = uiPositions[i].GetComponentInChildren<CenterPoint>();
            if (centerPoint == null)
            {
                Log.Error($"{enemies[i].Name} is missing a CenterPoint");
                centerPoints.Add(Vector3.zero);
            }
            centerPoints.Add(centerPoint.transform.position);
        }
    }

    public void Add(Enemy e, Transform uiPosition)
    {
        enemies.Add(e);
        uiPositions.Add(uiPosition);
        var centerPoint = uiPosition.GetComponentInChildren<CenterPoint>();
        if (centerPoint == null)
        {
            Log.Error($"{e.Name} is missing a CenterPoint");
            centerPoints.Add(Vector3.zero);
        }
        centerPoints.Add(centerPoint.transform.position);
    }
    
    public void Clear() => enemies = new List<Enemy>();
}
