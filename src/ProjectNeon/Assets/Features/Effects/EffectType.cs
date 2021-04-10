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
    DamageOverTimeFormula = 84,

    AdjustStatMultiplicatively = 5,
    AdjustStatAdditivelyFormula = 68,
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
    AdjustPrimaryResource = 46,
    AdjustPrimaryResourceFormula = 82,
    DrainPrimaryResourceFormula = 35,
    Reload = 38,
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    ReactOnSpellshieldedWithCard = 78,
    [Obsolete("Not Actually an OnDeath Effect. This has a hack that PREVENTS death.")] OnDeath = 25,
    
    // Transformers
    DoubleTheEffectAndMinusDurationTransformer = 28,
    AddToXCostTransformer = 32,
    
    // Cards Effects
    PlayBonusCardAfterNoCardPlayedInXTurns = 76,
    RedrawHandOfCards = 77,
    DrawCards = 81,
    GlitchRandomCards = 33,
    
    // Statuses
    DisableForTurns = 13,
    ApplyVulnerable = 10,
    EnterStealth = 58,
    GainDoubleDamage = 57,
    DuplicateStatesOfType = 65,
    
    // Healing
    HealFormula = 29,
    AntiHeal = 60,
    HealOverTime = 20,
    HealMagic = 45,
    HealToughness = 12,
    HealPercentMissingHealth = 56,
    FullyReviveAllAllies = 61,
    
    // Party Effects
    AdjustPlayerStats = 49,
    GainCredits = 52,
    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    
    // Miscellaneous
    SwapLifeForce = 64,
    Kill = 24,
    ShowCustomTooltip = 73,
    LeaveBattle = 34,
    AdjustCardTagPrevention = 37,
}
