// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// LAST NUMBER USED: 111

public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    AttackFormula = 30,
    MagicAttackFormula = 31,
    TrueDamageAttackFormula = 95,
    DealTrueDamageFormula = 70,
    DamageOverTimeFormula = 84,

    AdjustStatAdditivelyFormula = 68,
    AdjustStatMultiplicativelyFormula = 85,
    AdjustCounterFormula = 79,

    // Timing
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,
    PlayCardAtStartOfTurn = 104,

    // Resets
    RemoveDebuffs = 4,
    ResetStatToBase = 83,
    RemoveDots = 109,
    
    // Shields
    ShieldFormula = 80,
    ShieldRemoveAll = 22,
    ShieldBasedOnNumberOfOpponentsDoTs = 96,
    
    // Resources
    AdjustResourceFlat = 7,
    AdjustPrimaryResourceFormula = 82,
    TransferPrimaryResourceFormula = 35,
    Reload = 38,
    AdjustCostOfAllCardsInHandAtEndOfTurn = 39, //once we have more cost modifying cards we can build a more generic type
    AdjustCardCosts = 94,
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    ReactOncePerTurnWithEffect = 108,
    OnDeath = 25,
    ReactWithEnoughHealthLost = 111,

    // Transformers
    AddToXCostTransformer = 32,
    
    // Cards Effects
    PlayBonusCardAfterNoCardPlayedInXTurns = 76,
    PlayBonusChainCard = 92,
    CycleAllCardsInHand = 77,
    DrawCards = 81,
    DrawCardsOfOwner = 88,
    DrawCardsOfArchetype = 89,
    GlitchRandomCards = 33,
    FillHandWithOwnersCards = 41,
    ChooseAndDrawCard = 42,
    ChooseCardToCreate = 43,
    ChooseAndDrawCardOfArchetype = 87,
    ChooseCardToPlay = 110,

    // Statuses
    DisableForTurns = 13,
    EnterStealth = 58,
    ExitStealth = 107,
    GainDoubleDamage = 57,
    DuplicateStatesOfType = 65,
    DuplicateStatesOfTypeToRandomEnemy = 90,
    RemoveTemporalModsMatchingStatusTag = 99,
    InvulnerableForTurns = 100,
    
    // Healing
    HealFormula = 29,
    HealOverTime = 20,
    FullyReviveAllAllies = 61,

    // Party Effects
    AdjustPlayerStats = 49,
    AdjustPlayerStatsFormula = 28,
    GainCredits = 52,
    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    
    //Variables
    RecordConditionIsTrue = 102,
    AdjustScopedVariable = 103,
    
    // Miscellaneous
    Drain = 97,
    SwapLifeForce = 64,
    Kill = 24,
    ShowCustomTooltip = 73,
    LeaveBattle = 34,
    ResolveInnerEffect = 86,
    AdjustPrimaryStatForEveryCardCycledAndInHand = 40,
    BuyoutEnemyById = 47,
    ChooseBuyoutCardsOrDefault = 48,
    AdjustBattleRewardFormula = 91,
    TransformCardsIntoCard = 93,
    AdjustOwnersPrimaryResourceBasedOnTargetShieldSum = 98,
    RandomEffect = 101,
    RandomizeEnemyPosition = 105,
    MakeTargetingRough = 106,
}
