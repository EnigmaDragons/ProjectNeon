using UnityEngine;

public sealed class EnterSpecificBattle
{
    public GameObject BattleField { get; }
    public bool IsElite { get; }
    public EnemyInstance[] Enemies { get; }
    public bool IsStoryEventCombat { get; }
    public CardType[] OverrideDeck { get; }
    public bool IsTutorial { get; }
    public bool ShouldOverrideStartingCards { get; }
    public int OverrideNumStartingCards { get; }
    public bool AllowBasic { get; }

    public EnterSpecificBattle(GameObject battleField, bool isElite, EnemyInstance[] enemies, bool isStoryEventCombat, CardType[] overrideDeck, bool isTutorial = false, bool shouldOverrideStartingCards = false, int overrideNumStartingCards = -1, bool allowBasic = true)
    {
        BattleField = battleField;
        IsElite = isElite;
        Enemies = enemies;
        IsStoryEventCombat = isStoryEventCombat;
        OverrideDeck = overrideDeck;
        IsTutorial = isTutorial;
        ShouldOverrideStartingCards = shouldOverrideStartingCards;
        OverrideNumStartingCards = overrideNumStartingCards;
        AllowBasic = allowBasic;
    }
}
