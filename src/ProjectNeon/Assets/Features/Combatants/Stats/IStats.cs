﻿
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IStats
{
    float this[StatType statType] { get; }
    float this[TemporalStatType statType] { get; }
    IResourceType[] ResourceTypes { get; }
}

public static class StatsExtensions
{
    private static int RoundUp(this float v) => Mathf.CeilToInt(v);
    public static int MissingHp(this IStats stats) => stats.MaxHp() - stats.Hp();
    public static int MaxHp(this IStats stats) => stats[StatType.MaxHP].RoundUp();
    public static int Hp(this IStats stats) => stats[TemporalStatType.HP].RoundUp();
    public static int Shield(this IStats stats) => stats[TemporalStatType.Shield].RoundUp();
    public static int HpAndShield(this IStats stats) => stats.Hp() + stats.Shield();
    public static int StartingShield(this IStats stats) => stats[StatType.StartingShield].RoundUp();
    public static int MaxShield(this IStats stats) => stats[StatType.MaxShield].RoundUp();
    public static int Attack(this IStats stats) => stats[StatType.Attack].RoundUp();
    public static int Magic(this IStats stats) => stats[StatType.Magic].RoundUp();
    public static int Leadership(this IStats stats) => stats[StatType.Leadership].RoundUp();
    public static int Economy(this IStats stats) => stats[StatType.Economy].RoundUp();
    public static int Armor(this IStats stats) => stats[StatType.Armor].RoundUp();
    public static int Resistance(this IStats stats) => stats[StatType.Resistance].RoundUp();
    public static int ExtraCardPlays(this IStats stats) => stats[StatType.ExtraCardPlays].RoundUp();
    public static float Damagability(this IStats stats) => stats[StatType.Damagability];
    public static float Healability(this IStats stats) => stats[StatType.Healability];

    public static StatType[] PrimaryStatOptions => new[]
    {
        StatType.Attack,
        StatType.Magic,
        StatType.Leadership,
        StatType.Economy
    };

    public static StatType DefaultPrimaryStat(this IStats stats)
        => DefaultPrimaryStat(stats, stats);
    
    public static StatType DefaultPrimaryStat(this IStats stats, IStats baseStats) 
        => PrimaryStatOptions
            .OrderByDescending(x => stats[x])
            .ThenByDescending(x => baseStats[x])
            .First();

    public static HashSet<StatType> KeyStatTypes(this IStats stats)
        => PrimaryStatOptions.Where(x => stats[x] > 0).ToHashSet();

    public static IStats WithConvertedPower(this IStats stats, StatType primaryStat)
    {
        if (stats == null)
            return new StatAddends();
        
        var powerAmount = stats[StatType.Power];
        return new AddedStats(stats,
            new StatAddends().With(primaryStat, powerAmount).With(StatType.Power, -powerAmount));
    }
}
