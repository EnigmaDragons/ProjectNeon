using System.Collections.Generic;

public enum StatType
{
    MaxHP = 0,
    Attack = 2,
    Magic = 3,
    Armor = 4,
    Resistance = 5,
    Damagability = 6,
    Healability = 7,
    Leadership = 8,
    MaxShield = 9,
    StartingShield = 10,
    Economy = 11,
    ExtraCardPlays = 30,
}

public static class StatExtensions
{
    public static Dictionary<string, bool> _map = new Dictionary<string, bool>
    {
        {StatType.MaxHP.ToString(), true},
        {StatType.Attack.ToString(), true},
        {StatType.Magic.ToString(), true},
        {StatType.Armor.ToString(), true},
        {StatType.Resistance.ToString(), true},
        {StatType.Damagability.ToString(), false},
        {StatType.Healability.ToString(), true},
        {StatType.Leadership.ToString(), true},
        {StatType.MaxShield.ToString(), true},
        {StatType.StartingShield.ToString(), true},
        {StatType.Economy.ToString(), true},
        {StatType.ExtraCardPlays.ToString(), true},
        {TemporalStatType.HP.ToString(), true},
        {TemporalStatType.Shield.ToString(), true},
        {TemporalStatType.Disabled.ToString(), false},
        {TemporalStatType.Stun.ToString(), false},
        {TemporalStatType.Taunt.ToString(), true},
        {TemporalStatType.DoubleDamage.ToString(), true},
        {TemporalStatType.Stealth.ToString(), true},
        {TemporalStatType.Confused.ToString(), false},
        {TemporalStatType.Blind.ToString(), false},
        {TemporalStatType.Injury.ToString(), false},
        {TemporalStatType.Lifesteal.ToString(), true},
        {TemporalStatType.Phase.ToString(), true},
        {TemporalStatType.Inhibit.ToString(), false},
        {TemporalStatType.Dodge.ToString(), true},
        {TemporalStatType.Aegis.ToString(), true},
        {TemporalStatType.PreventDeath.ToString(), true},
        {TemporalStatType.Prominent.ToString(), false},
        {TemporalStatType.Marked.ToString(), false},
        {TemporalStatType.PreventResourceGains.ToString(), false},
        {TemporalStatType.Vulnerable.ToString(), false},
        {TemporalStatType.AntiHeal.ToString(), false},
    };

    public static Dictionary<string, bool> _isScalingStatMap = new Dictionary<string, bool>
    {
        {StatType.MaxHP.ToString(), false},
        {StatType.Attack.ToString(), true},
        {StatType.Magic.ToString(), true},
        {StatType.Armor.ToString(), false},
        {StatType.Resistance.ToString(), false},
        {StatType.Damagability.ToString(), false},
        {StatType.Healability.ToString(), false},
        {StatType.Leadership.ToString(), true},
        {StatType.MaxShield.ToString(), false},
        {StatType.StartingShield.ToString(), false},
        {StatType.Economy.ToString(), true},
        {StatType.ExtraCardPlays.ToString(), false},
        {TemporalStatType.HP.ToString(), false},
        {TemporalStatType.Shield.ToString(), false},
        {TemporalStatType.Disabled.ToString(), false},
        {TemporalStatType.Stun.ToString(), false},
        {TemporalStatType.Taunt.ToString(), false},
        {TemporalStatType.DoubleDamage.ToString(), false},
        {TemporalStatType.Stealth.ToString(), false},
        {TemporalStatType.Confused.ToString(), false},
        {TemporalStatType.Blind.ToString(), false},
        {TemporalStatType.Injury.ToString(), false},
        {TemporalStatType.Lifesteal.ToString(), false},
        {TemporalStatType.Phase.ToString(), false},
        {TemporalStatType.Inhibit.ToString(), false},
        {TemporalStatType.Dodge.ToString(), false},
        {TemporalStatType.Aegis.ToString(), false},
        {TemporalStatType.PreventDeath.ToString(), false},
        {TemporalStatType.Prominent.ToString(), false},
        {TemporalStatType.Marked.ToString(), false},
        {TemporalStatType.PreventResourceGains.ToString(), false},
        {TemporalStatType.Vulnerable.ToString(), false},
        {TemporalStatType.AntiHeal.ToString(), false},
    };

    public static bool IsScalingStat(string stat) => _map[stat];
    public static bool IsPositive(string stat) => _map[stat];
}