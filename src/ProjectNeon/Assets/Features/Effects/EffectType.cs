// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// NUMBERS AVAILABLE:
// LAST NUMBER USED: 87

public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    AttackFormula = 30,
    MagicAttackFormula = 31,
    DealRawDamageFormula = 70,
    DamageOverTimeFormula = 84,

    AdjustStatAdditivelyFormula = 68,
    AdjustStatMultiplicativelyFormula = 85,
    AdjustCounterFormula = 79,

    // Timing
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,

    // Resets
    RemoveDebuffs = 4,
    ResetStatToBase = 83,
    
    // Shields
    ShieldFormula = 80,
    ShieldRemoveAll = 22,
    ShieldToughnessBasedOnNumberOfOpponentDoTs = 69,
    
    // Resources
    AdjustResourceFlat = 7,
    AdjustPrimaryResourceFormula = 82,
    TransferPrimaryResourceFormula = 35,
    Reload = 38,
    AdjustCostOfAllCardsInHandAtEndOfTurn = 39, //once we have more cost modifying cards we can build a more generic type
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    OnDeath = 25,

    // Transformers
    AddToXCostTransformer = 32,
    
    // Cards Effects
    PlayBonusCardAfterNoCardPlayedInXTurns = 76,
    CycleAllCardsInHand = 77,
    DrawCards = 81,
    GlitchRandomCards = 33,
    FillHandWithOwnersCards = 41,
    DrawSelectedCard = 42,
    ChooseCardToCreate = 43,
    DrawCardOfArchetype = 87,

    // Statuses
    DisableForTurns = 13,
    ApplyVulnerable = 10,
    EnterStealth = 58,
    GainDoubleDamage = 57,
    DuplicateStatesOfType = 65,
    PreventResourceGains = 53,
    
    // Healing
    HealFormula = 29,
    HealOverTime = 20,
    FullyReviveAllAllies = 61,
    AntiHeal = 60,
    
    // Party Effects
    AdjustPlayerStats = 49,
    AdjustPlayerStatsFormula = 28,
    GainCredits = 52,
    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    
    // Miscellaneous
    SwapLifeForce = 64,
    Kill = 24,
    ShowCustomTooltip = 73,
    LeaveBattle = 34,
    AdjustCardTagPrevention = 37,
    ResolveInnerEffect = 86,
    AdjustPrimaryStatForEveryCardCycledAndInHand = 40,
    BuyoutEnemyById = 47,
    ChooseBuyoutCardsOrDefault = 48,
}
