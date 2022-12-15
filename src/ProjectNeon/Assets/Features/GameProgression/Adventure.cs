﻿using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject, CurrentAdventureData, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField] public string lockConditionExplanation = "";
    [SerializeField] private Adventure[] prerequisiteCompletedAdventures;
    [SerializeField] private AdventureMode mode;
    [SerializeField] private DynamicStage[] dynamicStages;
    [SerializeField] private StaticStageV4[] stages;
    [SerializeField] private HybridStageV5[] v5Stages;
    [SerializeField] public string adventureTitle;
    [SerializeField] public string mapAdventureTitle;
    [SerializeField] private Sprite adventureImage;
    [SerializeField] private int partySize;
    [SerializeField] public string allowedHeroesDescription = "";
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
    [SerializeField, TextArea(4, 10)] public string story;
    [SerializeField, TextArea(4, 10)] public string defeatConclusion = "";
    [SerializeField, TextArea(4, 10)] public string victoryConclusion = "";
    [SerializeField] private bool shouldRollCreditsBeforeConclusionScene = false;
    [SerializeField] private bool mapDeckbuildingEnabled = true;
    [SerializeField] private bool allowDifficultySelection = true;

    public AdventureMode Mode => mode;
    public int Id => id;
    public string TitleTerm => $"Adventures/Adventure{id}Title";
    public string RawMapTitleTerm => $"Adventures/Adventure{id}MapTitle";
    public string MapTitleTerm => string.IsNullOrWhiteSpace(RawMapTitleTerm.ToEnglish()) ? TitleTerm : RawMapTitleTerm;
    public string StoryTerm => $"Adventures/Adventure{id}Story";

    public string DefeatConclusionTerm => $"Adventures/Adventure{id}ConclusionDefeat";
    public string VictoryConclusionTerm => $"Adventures/Adventure{id}ConclusionVictory";
    public bool ShouldRollCreditsBeforeConclusionScene => shouldRollCreditsBeforeConclusionScene;

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

    public string LockConditionExplanationTerm => $"Adventures/Adventure{id}LockCondition";
    public bool IsLocked => !string.IsNullOrWhiteSpace(LockConditionExplanation) || prerequisiteCompletedAdventures.Any(p => !p.IsCompleted);
    public string LockConditionExplanation
    {
        get
        {
            var staticCondition = LockConditionExplanationTerm.ToLocalized() ?? "";
            if (staticCondition.Length > 0)
                return staticCondition;

            var firstUncompletedRequiredAdventure = prerequisiteCompletedAdventures.Where(p => !p.IsCompleted).FirstAsMaybe();
            return firstUncompletedRequiredAdventure.Select(a => string.Format("Adventures/DefaultLockedReason".ToLocalized(), a.MapTitleTerm.ToLocalized()), () => "");
        }
    }
    
    public string AllowedHeroesDescriptionTerm => $"Adventures/Adventure{id}AllowedHeroes";
    public BattleRewards NormalBattleRewards => normalBattleRewards;
    public BattleRewards EliteBattleRewards => eliteBattleRewards;
    public bool AllowDifficultySelection => allowDifficultySelection;

    public string[] GetLocalizeTerms() => new[]
    {
        TitleTerm,
        StoryTerm, 
        DefeatConclusionTerm, 
        VictoryConclusionTerm, 
        MapTitleTerm, 
        AllowedHeroesDescriptionTerm,
        LockConditionExplanationTerm, 
        RawMapTitleTerm,
        "Adventures/DefaultLockedReason"
    };
}
