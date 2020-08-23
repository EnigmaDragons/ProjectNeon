public enum EffectType
{
    Nothing = 0,
    HealFlat = 1,
    PhysicalDamage = 2,
    AdjustStatAdditively = 3,
    RemoveDebuffs = 4,
    AdjustStatMultiplicatively = 5,
    ShieldFlat = 6,
    ResourceFlat = 7,
    DamageOverTimeFlat = 8,
    OnEvaded = 9,
    ApplyVulnerable = 10,
    ShieldToughness = 11,
    AdjustTemporaryStatAdditively = 12,
    StunForTurns = 13,
    
    
    StealLifeNextAttack = 16,
    Attack = 17,
    InterceptAttackForTurns = 18,
    EvadeAttacks = 19,
    HealOverTime = 20,
    OnShieldBroken = 21,
    RemoveShields = 22,
    RandomizeTarget = 23,
    RepeatEffect = 24,
    AnyTargetHealthBelowThreshold = 25,
    OnAttacked = 26,
    CostResource = 27,
    ForNumberOfTurns = 28,
    
    ShieldBasedOnShieldValue = 30,
    ExcludeSelfFromEffect = 31,
    RepeatUntilPrimaryResourceDepleted = 32,
    SpellFlatDamageEffect = 33,
    OnNextTurnEffect = 34,
    EffectOnTurnStart = 36,
    TriggerFeedEffects = 37,
    HealPrimaryResource = 39,
    ApplyOnShieldBelowValue = 40,
    ApplyOnChance = 41,
    ReplayLastCard = 42,

    StunForNumberOfCards = 44,
    HealMagic = 45,
    GivePrimaryResource = 46,

    
    
    AdjustPlayerStats = 49,
}
