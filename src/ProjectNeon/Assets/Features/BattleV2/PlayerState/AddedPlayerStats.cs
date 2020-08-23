using System.Collections.Generic;
using System.Linq;

public class AddedPlayerStats : IPlayerStats
{
    private readonly IPlayerStats _first;
    private readonly IPlayerStats _second;
    
    public float this[PlayerStatType statType] => _first[statType] + _second[statType];
    
    public AddedPlayerStats(IPlayerStats first, IPlayerStats second)
    {
        _first = first;
        _second = second;
    }
}

public static class AddedPlayerStatExtensions
{
    public static IPlayerStats Plus(this IPlayerStats first, params IPlayerStats[] others) => Plus(first, (IEnumerable<IPlayerStats>) others);
    public static IPlayerStats Plus(this IPlayerStats first, IEnumerable<IPlayerStats> others) => others.Aggregate(first, (current, other) => new AddedPlayerStats(current, other));
}