public enum ReactionConditionType
{
    WhenAttacked = 0,
    WhenDamaged = 4,
    WhenDamagedHp = 1,
    WhenDamagedShield = 3,
    WhenDotted = 7,
    WhenBlinded = 2,
    WhenVulnerabled = 5,
    WhenShielded = 39,
    WhenShieldBroken = 6,
    WhenBloodied = 10,
    WhenGainedPrimaryResource = 11,
    WhenStunned = 106,
    WhenStatsReduced = 107,
    WhenMarked = 108,
    OnCausedAffliction = 14,
    OnCausedBloodied = 16,
    OnCausedHeal = 15,
    OnCausedStun = 20,
    OnCausedShieldGain = 37,
    OnSlay = 21,
    OnDamageDealt = 22,
    OnHpDamageDealt = 23,
    OnAppliedMark = 24,
    OnStealthed = 25,
    OnClipUsed = 27,
    OnTagCardPlayed = 29,
    OnDodged = 30,
    OnAegised = 31,
    WhenNearDeath = 32,
    WhenAllyDeath = 33,
    WhenAfflicted = 34,
    OnArchetypeCardPlayed = 35,
    OnCardPlayed = 36,
    OnNonQuickCardPlayed = 50,
    OnTeamCardCycled = 61,
    OnArchetypeCardDrawn = 62,
    WhenShieldMaxed = 38,
    WhenEnemyPrimaryStatBuffed = 40,
    WhenAllyVulnerable = 41,
    OnResourcesLost = 42,
    WhenEnemyShielded = 43,
    WhenEnemyStealthed = 109,
    WhenEnemyTaunted = 110,
    WhenEnemyGainedDodge = 111,
    WhenEnemyGainedAegis = 112,
    WhenEnemyGainedLifesteal = 113,
    WhenEnemyGainedResources = 114,
    WhenAllyBloodied = 44,
    WhenKilled = 99,
    WhenNonSelfAllyBloodied = 101,
    WhenNonSelfAllyHpDamaged = 102,
    WhenNonSelfAllyHpDamagedButNotKilled = 103,
    WhenNonSelfAllyBloodiedButNotKilled = 104,
    WhenHas10CardsOrMoreInHand = 105,
}
