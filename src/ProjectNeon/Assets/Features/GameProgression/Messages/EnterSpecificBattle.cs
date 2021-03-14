using UnityEngine;

public sealed class EnterSpecificBattle
{
    public GameObject BattleField { get; }
    public bool IsElite { get; }
    public Enemy[] Enemies { get; }

    public EnterSpecificBattle(GameObject battleField, bool isElite, Enemy[] enemies)
    {
        BattleField = battleField;
        IsElite = isElite;
        Enemies = enemies;
    }
}
