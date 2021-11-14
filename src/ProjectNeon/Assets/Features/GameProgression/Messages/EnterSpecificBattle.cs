using UnityEngine;

public sealed class EnterSpecificBattle
{
    public GameObject BattleField { get; }
    public bool IsElite { get; }
    public EnemyInstance[] Enemies { get; }
    public bool IsStoryEventCombat { get; }
    public bool IsTutorial { get; }

    public EnterSpecificBattle(GameObject battleField, bool isElite, EnemyInstance[] enemies, bool isStoryEventCombat, bool isTutorial = false)
    {
        BattleField = battleField;
        IsElite = isElite;
        Enemies = enemies;
        IsStoryEventCombat = isStoryEventCombat;
        IsTutorial = isTutorial;
    }
}
