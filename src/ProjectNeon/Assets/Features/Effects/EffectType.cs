// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// NUMBERS AVAILABLE: 29-43, 47, 48, 53, 
// LAST NUMBER USED: 73
public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    Attack = 17,
    DamageSpell = 50,
    DealRawDamageFormula = 70,
    
    AdjustStatMultiplicatively = 5,
    AdjustStatAdditivelyFormula = 68,
    AdjustCounter = 67,
    
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,
    
    PhysicalDamage = 2,
    RemoveDebuffs = 4,
    ShieldFlat = 6,
    ResourceFlat = 7,
    DamageOverTimeFlat = 8,
    DamageOverTime = 19,

    AntiHeal = 60,
    ApplyVulnerable = 10,
    StunForNumberOfCards = 44,
    EnterStealth = 58,
    
    // Reactions
    OnAttacked = 26,
    OnEvaded = 9,
    OnDeath = 25,
    
    // Transformers
    DoubleTheEffectAndMinusDuration = 28,
    
    ShieldToughness = 11,
    StunForTurns = 13,
    OnDamaged = 15,
    InterceptAttackForTurns = 18,
    HealOverTime = 20,
    OnShieldBroken = 21,
    RemoveShields = 22,
    HealMagic = 45,
    HealToughness = 12,
    AdjustPrimaryResource = 46,
    
    AdjustPlayerStats = 49,
    GainCredits = 52,
    
    ApplyTaunt = 51,
    MagicDamageOverTime = 55,
    HealPercentMissingHealth = 56,
    GainDoubleDamage = 57,
    PhysicalDamageOverTime = 59,
    FullyReviveAllAllies = 61,
    ApplyConfusion = 62,
    SwapLifeForce = 64,
    DuplicateStatesOfType = 65,
    ShieldToughnessBasedOnNumberOfOpponentDoTs = 69,
    Kill = 24,
    Suicide = 27,

    ApplyAdditiveStatInjury = 71,
    ApplyMultiplicativeStatInjury = 72,
    ShowCustomTooltip = 73,
}
