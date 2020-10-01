
// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER. EVENTUALLY WE NEED A TOOL TO FIND USAGES
public enum EffectType
{
    Nothing = 0,
    
    // Most Common Effects
    Attack = 17,
    DamageSpell = 50,
    
    AdjustStatAdditively = 3,
    AdjustStatMultiplicatively = 5,
    AdjustStatAdditivelyWithMagic = 63,
    
    AtStartOfTurn = 54,
    AtEndOfTurn = 66,
    
    PhysicalDamage = 2,
    RemoveDebuffs = 4,
    ShieldFlat = 6,
    ResourceFlat = 7,
    DamageOverTimeFlat = 8,
    
    EvadeAttacks = 19,
    AntiHeal = 60,
    ApplyVulnerable = 10,
    StunForNumberOfCards = 44,
    EnterStealth = 58,
    
    // Reactions
    OnAttacked = 26,
    OnEvaded = 9,
    
    ShieldToughness = 11,
    StunForTurns = 13,
    AdjustStatAdditivelyBaseOnMagicStat = 14,
    OnDamaged = 15,
    StealLifeNextAttack = 16,
    InterceptAttackForTurns = 18,
    HealOverTime = 20,
    OnShieldBroken = 21,
    RemoveShields = 22,
    HealMagic = 45,
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
}
