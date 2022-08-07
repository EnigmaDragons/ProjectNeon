using System;
using System.Collections.Generic;

public class StatsSnapshot : IStats
{
    private readonly Dictionary<StatType, float> _statValues = new Dictionary<StatType, float>();
    private readonly Dictionary<TemporalStatType, float> _temporalValues = new Dictionary<TemporalStatType, float>();

    public float this[StatType statType] => _statValues[statType];
    public float this[TemporalStatType statType] => _temporalValues[statType];
    public IResourceType[] ResourceTypes { get; }

    public StatsSnapshot(IResourceType[] resourceTypes, IStats origin, StatType primaryStat)
    {
        ResourceTypes = resourceTypes;
        foreach (StatType st in Enum.GetValues(typeof(StatType)))
            _statValues[st] = origin[st];
        _statValues[StatType.Power] = origin[primaryStat];
        foreach (TemporalStatType st in Enum.GetValues(typeof(TemporalStatType)))
            _temporalValues[st] = origin[st];
    }
}

public static class StatsSnapshotExtensions
{
    public static IStats ToSnapshot(this IStats stats, StatType primaryStat) => new StatsSnapshot(stats.ResourceTypes, stats, primaryStat);
}
