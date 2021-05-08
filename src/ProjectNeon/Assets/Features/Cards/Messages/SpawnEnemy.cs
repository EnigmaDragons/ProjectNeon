
using UnityEngine;

public class SpawnEnemy
{
    public Enemy Enemy { get; }
    public Vector3 Offset { get; } 

    public SpawnEnemy(Enemy e, Vector3 offset)
    {
        Enemy = e;
        Offset = offset;
    }
}
