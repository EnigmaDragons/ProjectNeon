using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject, CurrentAdventureData
{
    [SerializeField] public int id;
    [SerializeField] private string lockConditionExplanation = "";
    [SerializeField] private Adventure[] prerequisiteCompletedAdventures;
    [SerializeField] private AdventureMode mode;
    [SerializeField] private DynamicStage[] dynamicStages;
    [SerializeField] private StaticStageV4[] stages;
    [SerializeField] private HybridStageV5[] v5Stages;
    [SerializeField] private string adventureTitle;
    [SerializeField] private string mapAdventureTitle;
    [SerializeField] private Sprite adventureImage;
    [SerializeField] private int partySize;
    [SerializeField] private string allowedHeroesDescription = "";
    [SerializeField] private BaseHero[] requiredHeroes;
    [SerializeField] private BaseHero[] bannedHeroes;
    [SerializeField] private BaseHero[] fixedStartingHeroes;
    [SerializeField] private int baseNumberOfCardCycles = 2;
    [SerializeField] private float rewardCreditsPerPowerLevel = 1f;
    [SerializeField] private float xpPerPowerLevel = 0.2f;
    [SerializeField] private float bonusXpFactor = 0.33333f;
    [SerializeField] private int maxHeroLevel = 99;
    [SerializeField] private BattleRewards normalBattleRewards;
    [SerializeField] private BattleRewards eliteBattleRewards;
    [SerializeField] private int startingClinicVouchers = 1;
    [SerializeField] private int battleRewardClinicVouchers = 1;
    [SerializeField] private int numOfImplantOptions = 4;
    [SerializeField, TextArea(4, 10)] private string story;
    [SerializeField, TextArea(4, 10)] private string defeatConclusion = "";
    [SerializeField, TextArea(4, 10)] private string victoryConclusion = "";
    [SerializeField] private bool mapDeckbuildingEnabled = true;
    [SerializeField] private bool allowDifficultySelection = true;

    public AdventureMode Mode => mode;
    public int Id => id;
    public string Title => adventureTitle;
    public string MapTitle => string.IsNullOrWhiteSpace(mapAdventureTitle) ? adventureTitle : mapAdventureTitle;
    public string Story => story;

    public string DefeatConclusion => defeatConclusion;
    public string VictoryConclusion => victoryConclusion;

    public DynamicStage[] DynamicStages => dynamicStages.ToArray();
    public StaticStageV4[] StagesV4 => stages.ToArray();
    public HybridStageV5[] StagesV5 => v5Stages.ToArray();
    
    public Sprite AdventureImage => adventureImage;
    public int PartySize => partySize;
    public BaseHero[] RequiredHeroes => requiredHeroes;
    public BaseHero[] BannedHeroes => bannedHeroes ?? Array.Empty<BaseHero>();
    public BaseHero[] FixedStartingHeroes => fixedStartingHeroes ?? Array.Empty<BaseHero>();
    public int BaseNumberOfCardCycles => baseNumberOfCardCycles;
    public float RewardCreditsPerPowerLevel => rewardCreditsPerPowerLevel;
    public float XpPerPowerLevel => xpPerPowerLevel;
    public float BonusXpFactor => bonusXpFactor;
    public int StartingClinicVouchers => startingClinicVouchers;
    public int BattleRewardClinicVouchers => battleRewardClinicVouchers;
    public int NumOfImplantOptions => numOfImplantOptions;
    public int MaxHeroLevel => maxHeroLevel;
    public bool IsV1 => false;
    public bool IsV2 => !IsV4 && !IsV5 && dynamicStages != null && dynamicStages.Any();
    public bool IsV4 => stages != null && stages.Any();
    public bool IsV5 => v5Stages != null && v5Stages.Any();

    public bool IsCompleted => CurrentProgressionData.Data.Completed(Id);
    public bool MapDeckbuildingEnabled => mapDeckbuildingEnabled;
    
    public bool IsLocked => !string.IsNullOrWhiteSpace(lockConditionExplanation) || prerequisiteCompletedAdventures.Any(p => !p.IsCompleted);
    public string LockConditionExplanation
    {
        get
        {
            var staticCondition = lockConditionExplanation ?? "";
            if (staticCondition.Length > 0)
                return staticCondition;

            var firstUncompletedRequiredAdventure = prerequisiteCompletedAdventures.Where(p => !p.IsCompleted).FirstAsMaybe();
            return firstUncompletedRequiredAdventure.Select(a => $"Must Complete {a.MapTitle}", () => "");
        }
    }

    public string AllowedHeroesDescription => allowedHeroesDescription;
    public BattleRewards NormalBattleRewards => normalBattleRewards;
    public BattleRewards EliteBattleRewards => eliteBattleRewards;
    public bool AllowDifficultySelection => allowDifficultySelection;
}
