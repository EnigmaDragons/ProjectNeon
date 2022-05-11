using UnityEngine;

public class EnemySpawnDetails
{
    public EnemyInstance Enemy { get; }
    public Member Member { get; }
    public Transform Transform { get; }

    public EnemySpawnDetails(EnemyInstance e, Member m, Transform t)
    {
        Enemy = e;
        Member = m;
        Transform = t;
    }
}
