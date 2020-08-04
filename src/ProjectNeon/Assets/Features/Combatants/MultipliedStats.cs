using System.Collections.Generic;
using System.Linq;

public sealed class MultipliedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;
    private readonly ResourceTypeStats _resourceTypeStats;

    public float this[StatType statType] => _first[statType] * _second[statType];
    public float this[TemporalStatType statType] => _first[statType] * _second[statType];

    public IResourceType[] ResourceTypes => _resourceTypeStats.AsArray();

    public MultipliedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
        _resourceTypeStats = new ResourceTypeStats()
            .WithAdded(first.ResourceTypes)
            .WithMultiplied(second.ResourceTypes);
    }
}

public static class MultipliedStatExtensions
{
    public static IStats Times(this IStats first, params IStats[] others) => Times(first, (IEnumerable<IStats>) others);
    public static IStats Times(this IStats first, IEnumerable<IStats> others) => others.Aggregate(first, (current, other) => new MultipliedStats(current, other));
}
