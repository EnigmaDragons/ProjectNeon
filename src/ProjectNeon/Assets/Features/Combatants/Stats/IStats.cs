
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
    public static int Toughness(this IStats stats) => stats[StatType.Toughness].RoundUp();
    public static int StartingShield(this IStats stats) => stats[StatType.StartingShield].RoundUp();
    public static int MaxShield(this IStats stats) => stats[StatType.MaxShield].RoundUp();
    public static int Attack(this IStats stats) => stats[StatType.Attack].RoundUp();
    public static int Magic(this IStats stats) => stats[StatType.Magic].RoundUp();
    public static int Leadership(this IStats stats) => stats[StatType.Leadership].RoundUp();
    public static int Armor(this IStats stats) => stats[StatType.Armor].RoundUp();
    public static int Resistance(this IStats stats) => stats[StatType.Resistance].RoundUp();
    public static int ExtraCardPlays(this IStats stats) => stats[StatType.ExtraCardPlays].RoundUp();
    public static float Damagability(this IStats stats) => stats[StatType.Damagability];
    public static float Healability(this IStats stats) => stats[StatType.Healability];
}
