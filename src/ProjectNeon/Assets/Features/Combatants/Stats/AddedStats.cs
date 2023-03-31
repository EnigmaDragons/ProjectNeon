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

public sealed class SubtractedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;
    private readonly ResourceTypeStats _resourceTypeStats;
    
    public float this[StatType statType] => _first[statType] - _second[statType];

    public float this[TemporalStatType statType] => _first[statType] - _second[statType];

    public IResourceType[] ResourceTypes => _resourceTypeStats.AsArray();

    public SubtractedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
        _resourceTypeStats = new ResourceTypeStats()
            .WithAdded(_first.ResourceTypes)
            .WithSubtracted(_second.ResourceTypes);
    }
}

public static class AddedStatExtensions
{
    public static IStats Plus(this IStats first, params IStats[] others)
    {
        if (first == null || others.AnyNonAlloc(x => x == null))
            Log.NonCrashingError("One or more Stats passed to Plus were null");
        return first == null 
            ? Plus(new StatAddends(), (IEnumerable<IStats>)others) 
            : Plus(first, (IEnumerable<IStats>)others);
    }

    public static IStats Plus(this IStats first, IEnumerable<IStats> others) => others.Where(o => o != null).Aggregate(first, (current, other) => new AddedStats(current, other));
    public static IStats Minus(this IStats first, IStats second) => new SubtractedStats(first, second);
}
