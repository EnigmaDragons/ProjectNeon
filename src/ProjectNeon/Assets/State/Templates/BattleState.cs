using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private EnemyArea enemies;
    
    // @todo #1:10min Replace this with Member once implemented.
    [SerializeField] private Enemy tempTarget;
}
