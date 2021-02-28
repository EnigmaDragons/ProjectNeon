// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// NUMBERS AVAILABLE: 35-43, 47, 48, 53, 
// LAST NUMBER USED: 82

using System;

public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    Attack = 17,
    AttackFormula = 30,
    MagicAttack = 50,
    MagicAttackFormula = 31,
    DealRawDamageFormula = 70,

    AdjustStatMultiplicatively = 5,
    AdjustStatAdditivelyFormula = 68,
    AdjustCounterFormula = 79,
    
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,
    
    RemoveDebuffs = 4,
    ShieldFormula = 80,
    ShieldRemoveAll = 22,
    ShieldToughnessBasedOnNumberOfOpponentDoTs = 69,
    AdjustResourceFlat = 7,
    AdjustPrimaryResource = 46,
    AdjustPrimaryResourceFormula = 82,
    DamageOverTimeFlat = 8,
    DamageOverTime = 19,
    PhysicalDamageOverTime = 59,
    HealFormula = 29,

    AntiHeal = 60,
    ApplyVulnerable = 10,
    EnterStealth = 58,
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    ReactOnEvadedWithCard = 9,
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
    InterceptAttackForTurns = 18,
    HealOverTime = 20,
    HealMagic = 45,
    HealToughness = 12,
    HealPercentMissingHealth = 56,
    
    AdjustPlayerStats = 49,
    GainCredits = 52,
    
    MagicDamageOverTime = 55,
    GainDoubleDamage = 57,
    FullyReviveAllAllies = 61,
    ApplyConfusion = 62,
    SwapLifeForce = 64,
    DuplicateStatesOfType = 65,
    Kill = 24,

    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    ShowCustomTooltip = 73,
    GlitchRandomCards = 33,
    LeaveBattle = 34,
}
