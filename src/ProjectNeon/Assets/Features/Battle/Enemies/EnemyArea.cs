using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyArea : ScriptableObject
{
    [SerializeField] public List<Enemy> enemies;
}
