// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// NUMBERS AVAILABLE: 39-43, 47, 48, 53, 
// LAST NUMBER USED: 84

using System;

public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    AttackFormula = 30,
    MagicAttackFormula = 31,
    DealRawDamageFormula = 70,
    
    // Damage Over Time
    DamageOverTimeFormula = 84,
    DamageOverTimeFlat = 8,
    DamageOverTime = 19,
    PhysicalDamageOverTime = 59,
    MagicDamageOverTime = 55,

    AdjustStatMultiplicatively = 5,
    AdjustStatAdditivelyFormula = 68,
    AdjustCounterFormula = 79,
    AdjustPrimaryStatAdditivelyFormula = 36,

    // Timing
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,
    
    // Cleanses
    RemoveDebuffs = 4,
    ResetStatToBase = 83,
    
    // Shields
    ShieldFormula = 80,
    ShieldRemoveAll = 22,
    ShieldToughnessBasedOnNumberOfOpponentDoTs = 69,
    
    // Resources
    AdjustResourceFlat = 7,
    AdjustPrimaryResource = 46,
    AdjustPrimaryResourceFormula = 82,
    DrainPrimaryResourceFormula = 35,
    
    HealFormula = 29,

    AntiHeal = 60,
    ApplyVulnerable = 10,
    EnterStealth = 58,
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    ReactOnSpellshieldedWithCard = 78,
    [Obsolete("Not Actually an OnDeath Effect. This has a hack that PREVENTS death.")] OnDeath = 25,
    
    // Transformers
    DoubleTheEffectAndMinusDurationTransformer = 28,
    AddToXCostTransformer = 32,
    
    // Bonus Cards
    PlayBonusCardAfterNoCardPlayedInXTurns = 76,
    RedrawHandOfCards = 77,
    DrawCards = 81,
    
    DisableForTurns = 13,
    HealOverTime = 20,
    HealMagic = 45,
    HealToughness = 12,
    HealPercentMissingHealth = 56,
    
    AdjustPlayerStats = 49,
    GainCredits = 52,
    
    GainDoubleDamage = 57,
    FullyReviveAllAllies = 61,
    SwapLifeForce = 64,
    DuplicateStatesOfType = 65,
    Kill = 24,

    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    ShowCustomTooltip = 73,
    GlitchRandomCards = 33,
    LeaveBattle = 34,
    
    AdjustCardTagPrevention = 37,
    Reload = 38,
}
