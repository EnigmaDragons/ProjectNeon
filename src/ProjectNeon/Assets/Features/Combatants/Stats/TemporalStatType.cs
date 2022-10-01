using System;
using System.Collections.Generic;
using System.Linq;

public enum TemporalStatType
{
    HP = 0,
    Shield = 1,
    Disabled = 2,
    Stun = 3,
    Taunt = 6,
    DoubleDamage = 7,
    Stealth = 8,
    Confused = 9,
    Blind = 10,
    Injury = 11,
    Lifesteal = 12,
    Phase = 13,
    Inhibit = 14,
    Dodge = 15,
    Aegis = 16,
    PreventDeath = 17,
    Prominent = 18,
    Marked = 19,
    PreventResourceGains = 20,
    Vulnerable = 21,
    AntiHeal = 22,
    PrimaryResource = 23,
    OverkillDamageAmount = 24,
}

public static class TemporalStatTypeExtensions
{
    public static readonly TemporalStatType[] StatTypes = Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>().ToArray();
    public static readonly Dictionary<TemporalStatType, string> StatNames = StatTypes.ToDictionary(s => s, s => s.ToString());
    public static readonly Dictionary<string, TemporalStatType> StatTypesByName = StatTypes.ToDictionary(s => s.ToString(), s => s);
    
    public static string GetString(this TemporalStatType s) => StatNames[s];
}
