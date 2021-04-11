using UnityEngine;

public sealed class EnterSpecificBattle
{
    public GameObject BattleField { get; }
    public bool IsElite { get; }
    public EnemyInstance[] Enemies { get; }

    public EnterSpecificBattle(GameObject battleField, bool isElite, EnemyInstance[] enemies)
    {
        BattleField = battleField;
        IsElite = isElite;
        Enemies = enemies;
    }
}
