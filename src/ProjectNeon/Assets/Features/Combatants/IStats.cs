
public interface IStats
{
    float this[StatType statType] { get; }
    IResourceType[] ResourceTypes { get; }
}

public static class StatsExtensions
{
    public static float MaxHP(this IStats stats) => stats[StatType.MaxHP];
    public static float Toughness(this IStats stats) => stats[StatType.Toughness];
    public static float Attack(this IStats stats) => stats[StatType.Attack];
    public static float Magic(this IStats stats) => stats[StatType.Magic];
    public static float Armor(this IStats stats) => stats[StatType.Armor];
    public static float Resistance(this IStats stats) => stats[StatType.Resistance];
    public static float Damagability(this IStats stats) => stats[StatType.Damagability];
}
