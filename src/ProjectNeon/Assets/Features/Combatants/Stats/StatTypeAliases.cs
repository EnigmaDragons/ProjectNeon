using System;
using System.Collections.Generic;
using System.IO;

public static class StatTypeAliases
{
    public static readonly Dictionary<string, StatType> AbbreviationToStat = new Dictionary<string, StatType>(StringComparer.InvariantCultureIgnoreCase)
    {
        { "LEAD", StatType.Leadership },
        { "LD", StatType.Leadership },
        { "ATK", StatType.Attack },
        { "MAG", StatType.Magic },
        { "ARM", StatType.Armor },
        { "ECON", StatType.Economy },
        { "EC", StatType.Economy },
        { "POW", StatType.Power },
    };

    public static readonly Dictionary<string, string> AbbreviationToFullNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        { "LEAD", StatType.Leadership.ToString() },
        { "LD", StatType.Leadership.ToString() },
        { "ATK", StatType.Attack.ToString() },
        { "MAG", StatType.Magic.ToString() },
        { "ARM", StatType.Armor.ToString() },
        { "ECON", StatType.Economy.ToString() },
        { "EC", StatType.Economy.ToString() },
        { "POW", StatType.Power.ToString() },
    };

    public static readonly Dictionary<string, string> FullNameToAbbreviations = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        { StatType.Leadership.ToString(), "LD" },
        { StatType.Attack.ToString(), "ATK" },
        { StatType.Magic.ToString(), "MAG" },
        { StatType.Armor.ToString(), "ARM" },
        { StatType.Economy.ToString(), "EC" },
        { StatType.Power.ToString(), "POW" },
    };
}