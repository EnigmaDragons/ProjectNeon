using System.Collections.Generic;
using System.Linq;

public sealed class AddedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;
    private readonly ResourceTypeStats _resourceTypeStats;
    
    public float this[StatType statType] => _first[statType] + _second[statType];

    public float this[TemporalStatType statType] => _first[statType] + _second[statType];

    public IResourceType[] ResourceTypes => _resourceTypeStats.AsArray();

    public AddedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
        _resourceTypeStats = new ResourceTypeStats().WithAdded(_first.ResourceTypes).WithAdded(_second.ResourceTypes);
    }
}

public static class AddedStatExtensions
{
    public static IStats Plus(this IStats first, params IStats[] others) => Plus(first, (IEnumerable<IStats>) others);
    public static IStats Plus(this IStats first, IEnumerable<IStats> others) => others.Aggregate(first, (current, other) => new AddedStats(current, other));
}
