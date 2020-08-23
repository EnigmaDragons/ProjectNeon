using UnityEngine;

public interface IPlayerStats
{
    float this[PlayerStatType statType] { get; }
}

public static class PlayerStatsExtensions
{
    private static int RoundUp(this float v) => Mathf.CeilToInt(v);
    public static int CardPlays(this IPlayerStats stats) => stats[PlayerStatType.CardPlays].RoundUp();
    public static int CardDraw(this IPlayerStats stats) => stats[PlayerStatType.CardDraw].RoundUp();
    public static int CardCycles(this IPlayerStats stats) => stats[PlayerStatType.CardCycles].RoundUp();
}