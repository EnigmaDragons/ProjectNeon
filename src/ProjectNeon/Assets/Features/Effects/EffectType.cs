// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER.
// NUMBERS AVAILABLE: 33-43, 47, 48, 53, 
// LAST NUMBER USED: 80
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
    AdjustCounter = 67,
    AdjustCounterFormula = 79,
    
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    DelayedStartOfTurn = 23,
    
    PhysicalDamage = 2,
    RemoveDebuffs = 4,
    ShieldFlat = 6,
    ShieldToughness = 11,
    ShieldFormula = 80,
    AdjustResourceFlat = 7,
    DamageOverTimeFlat = 8,
    DamageOverTime = 19,
    HealFormula = 29,

    AntiHeal = 60,
    ApplyVulnerable = 10,
    StunForNumberOfCards = 44,
    EnterStealth = 58,
    
    // Reactions
    ReactWithEffect = 74,
    ReactWithCard = 75,
    ReactOnEvadedWithCard = 9,
    ReactOnSpellshieldedWithCard = 78,
    OnDeath = 25,
    
    // Transformers
    DoubleTheEffectAndMinusDurationTransformer = 28,
    AddToXCostTransformer = 32,
    
    // Bonus Cards
    PlayBonusCardAfterNoCardPlayedInXTurns = 76,
    RedrawHandOfCards = 77,
    
    StunForTurns = 13,
    InterceptAttackForTurns = 18,
    HealOverTime = 20,
    RemoveShields = 22,
    HealMagic = 45,
    HealToughness = 12,
    AdjustPrimaryResource = 46,
    
    AdjustPlayerStats = 49,
    GainCredits = 52,
    
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
