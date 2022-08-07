using System;
using System.Collections.Generic;
using System.Linq;

public enum StatusTag
{
    None = 0,
    CounterAttack = 1,
    DamageOverTime = 2,
    HealOverTime = 3,
    WhenHit = 5,
    StartOfTurnTrigger = 7,
    EndOfTurnTrigger = 8,
    WhenDamaged = 9,
    WhenKilled = 10,
    Invulnerable = 11,
    Augment = 12,
    Trap = 13,
    WhenBloodied = 14,
    Stealth = 15,
    OnHpDamageDealt = 16,
    WhenShieldBroken = 17,
    OnClipUsed = 18,
    WhenAllyKilled = 19,
    WhenAfflicted = 20,
    CurrentCardOnly = 21,
    WhenIgnited = 22,
    ResourceGainsPrevented = 23,
    AfterShielded = 24,
    RetainsStealth = 25
}

public static class StatusTagExtensions
{
    public static StatusTag[] Values = Enum.GetValues(typeof(StatusTag)).Cast<StatusTag>().ToArray();
    public static Dictionary<StatusTag, string> StatNames = Values.ToDictionary(s => s, s => s.ToString());
    public static Dictionary<string, StatusTag> StatTypesByName = Values.ToDictionary(s => s.ToString(), s => s);
    public static string GetString(this StatusTag s) => StatNames[s];
}
