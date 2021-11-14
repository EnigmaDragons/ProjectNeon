using UnityEngine;

public sealed class EnterSpecificBattle
{
    public GameObject BattleField { get; }
    public bool IsElite { get; }
    public EnemyInstance[] Enemies { get; }
    public bool IsStoryEventCombat { get; }
    public bool SkipSetup { get; }

    public EnterSpecificBattle(GameObject battleField, bool isElite, EnemyInstance[] enemies, bool isStoryEventCombat, bool skipSetup)
    {
        BattleField = battleField;
        IsElite = isElite;
        Enemies = enemies;
        IsStoryEventCombat = isStoryEventCombat;
        SkipSetup = skipSetup;
    }
}
