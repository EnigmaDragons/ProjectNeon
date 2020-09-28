using System;

// WARNING: BE VERY CAREFUL NOT TO RE-USE NUMBERS. ALSO, THESE AREN'T IN NUMERICAL ORDER. EVENTUALLY WE NEED A TOOL TO FIND USAGES
public enum EffectType
{
    Nothing = 0,
    Attack = 17,
    DamageSpell = 50,
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
    AdjustStatAdditivelyBaseOnMagicStat = 14,
    OnDamaged = 15,
    StealLifeNextAttack = 16,
    InterceptAttackForTurns = 18,
    EvadeAttacks = 19,
    HealOverTime = 20,
    OnShieldBroken = 21,
    RemoveShields = 22,
    //[Obsolete]RandomizeTarget = 23,
    //[Obsolete]RepeatEffect = 24,
    AnyTargetHealthBelowThreshold = 25,
    OnAttacked = 26,
    CostResource = 27,
    //[Obsolete]ForNumberOfTurns = 28,
    ShieldBasedOnShieldValue = 30,
    ExcludeSelfFromEffect = 31,
    //[Obsolete]RepeatUntilPrimaryResourceDepleted = 32,
    SpellFlatDamageEffect = 33,
    //[Obsolete]OnNextTurnEffect = 34,
    //[Obsolete]EffectOnTurnStart = 36,
    //[Obsolete]TriggerFeedEffects = 37,
    HealPrimaryResource = 39,
    ApplyOnShieldBelowValue = 40,
    ApplyOnChance = 41,
    //[Obsolete]ReplayLastCard = 42,
    StunForNumberOfCards = 44,
    HealMagic = 45,
    AdjustPrimaryResource = 46,
    AdjustPlayerStats = 49,
    ApplyTaunt = 51,
    GainCredits = 52,
    AtStartOfTurn = 54,
    MagicDamageOverTime = 55,
    HealPercentMissingHealth = 56,
    GainDoubleDamage = 57,
    EnterStealth = 58,
    PhysicalDamageOverTime = 59,
    AntiHeal = 60,
    FullyReviveAllAllies = 61,
    ApplyConfusion = 62,
    AdjustStatAdditivelyWithMagic = 63,
    SwapLifeForce = 64,
    DuplicateStatesOfType = 65,
}
