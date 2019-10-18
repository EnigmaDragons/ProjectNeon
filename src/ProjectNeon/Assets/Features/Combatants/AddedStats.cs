using System.Collections.Generic;

public sealed class AddedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;

    public int MaxHP => _first.MaxHP + _second.MaxHP;
    public int MaxShield => _first.MaxShield + _second.MaxShield;
    public int Attack => _first.Attack + _second.Attack;
    public int Magic => _first.Magic + _second.Magic;
    public float Armor => _first.Armor + _second.Armor;
    public float Resistance => _first.Resistance + _second.Resistance;
    
    // @todo #1:15min Combine MaxResource Amounts
    public IResourceType[] ResourceTypes => _first.ResourceTypes;

    public AddedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
    }
}

public static class AddedStatExtensions
{
    public static IStats Plus(this IStats first, params IStats[] others) => Plus(first, (IEnumerable<IStats>) others);
    public static IStats Plus(this IStats first, IEnumerable<IStats> others)
    {
        var result = first;
        foreach (var other in others)
            result = new AddedStats(result, other);
        return result;
    }
}
