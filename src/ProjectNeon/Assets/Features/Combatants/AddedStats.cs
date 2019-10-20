using System.Collections.Generic;
using System.Linq;

public sealed class AddedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;

    public float this[StatType statType] => _first[statType] + _second[statType];

    // @todo #1:15min Combine Added MaxResource Amounts
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
    public static IStats Plus(this IStats first, IEnumerable<IStats> others) => others.Aggregate(first, (current, other) => new AddedStats(current, other));
}
