using System;
using System.Collections.Generic;
using System.Linq;

public class StatsSnapshotMut : IStats
{
    private readonly Dictionary<StatType, float> _statValues = new Dictionary<StatType, float>(StatExtensions.StatTypes.Length);
    private readonly Dictionary<TemporalStatType, float> _temporalValues = new Dictionary<TemporalStatType, float>(TemporalStatTypeExtensions.StatTypes.Length);

    public float this[StatType statType] => _statValues[statType];
    public float this[TemporalStatType statType] => _temporalValues[statType];
    public IResourceType[] ResourceTypes { get; private set; }

    public StatsSnapshotMut() => ResourceTypes = Array.Empty<IResourceType>();

    public StatsSnapshotMut(IResourceType[] resourceTypes, IStats origin, StatType primaryStat) 
        => Initialized(resourceTypes, origin, primaryStat);

    public StatsSnapshotMut Initialized(IResourceType[] resourceTypes, IStats origin, StatType primaryStat)
    {
        ResourceTypes = resourceTypes;
        foreach (var st in StatExtensions.StatTypes)
            _statValues[st] = origin[st];
        _statValues[StatType.Power] = origin[primaryStat];
        foreach (var st in TemporalStatTypeExtensions.StatTypes)
            _temporalValues[st] = origin[st];
        return this;
    }
}

public static class StatsSnapshotExtensions
{
    private static int _poolIndex = 0;
    private static readonly StatsSnapshotMut[] Pool = Enumerable.Range(0, 1000).Select(_ => new StatsSnapshotMut()).ToArray();

    private static StatsSnapshotMut GetNext() => Pool[++_poolIndex];

    public static bool Init = true;
    
    public static IStats ToSnapshot(this IStats stats, StatType primaryStat) => GetNext().Initialized(stats.ResourceTypes, stats, primaryStat);
}
