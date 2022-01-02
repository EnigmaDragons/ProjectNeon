using System;
using System.Collections.Generic;

public class StatsSnapshot : IStats
{
    private readonly Dictionary<string, float> _values = new Dictionary<string, float>();

    public float this[StatType statType] => _values[statType.ToString()];
    public float this[TemporalStatType statType] => _values[statType.ToString()];
    public IResourceType[] ResourceTypes { get; }

    public StatsSnapshot(IResourceType[] resourceTypes, IStats origin)
    {
        ResourceTypes = resourceTypes;
        foreach (StatType st in Enum.GetValues(typeof(StatType)))
            _values[st.ToString()] = origin[st];
        foreach (TemporalStatType st in Enum.GetValues(typeof(TemporalStatType)))
            _values[st.ToString()] = origin[st];
    }
}

public static class StatsSnapshotExtensions
{
    public static IStats ToSnapshot(this IStats stats) => new StatsSnapshot(stats.ResourceTypes, stats);
}
