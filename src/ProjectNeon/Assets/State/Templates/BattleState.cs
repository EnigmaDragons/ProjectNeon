using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private EnemyArea enemies;
    
    // @todo #1:10min Replace this with Member once implemented.
    [SerializeField] private Member tempTarget;

    public Party Party => party;
    public EnemyArea EnemyArea => enemies;

    public BattleState Initialized(Party party, EnemyArea enemyArea)
    {
        this.party = party;
        this.enemies = enemyArea;
        return this;
    }
}
