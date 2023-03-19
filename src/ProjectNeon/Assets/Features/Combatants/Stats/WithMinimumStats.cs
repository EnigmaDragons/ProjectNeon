using System;
using System.Linq;
using UnityEngine;

public sealed class WithMinimumStats : IStats
{
    private float _minimum;
    private IStats _inner;
    private StatType[] _exceptions;

    public float this[StatType statType] => _exceptions.Contains(statType) ? _inner[statType] : Mathf.Max(_inner[statType], _minimum);
    public float this[TemporalStatType statType] => Mathf.Max(_inner[statType], _minimum);

    public IResourceType[] ResourceTypes => _inner.ResourceTypes;

    public WithMinimumStats() : this(0, new StatAddends(), Array.Empty<StatType>()) {}
    public WithMinimumStats(float minimum, IStats inner, StatType[] exceptions) => Initialized(minimum, inner, exceptions);

    public WithMinimumStats Initialized(float minimum, IStats inner, StatType[] exceptions)
    {
        _minimum = minimum;
        _inner = inner;
        _exceptions = exceptions;
        return this;
    }
}

public static class WithMinimumStatsExtensions
{
    private static int _poolIndex = 0;
    private static readonly WithMinimumStats[] Pool = Enumerable.Range(0, 500).Select(_ => new WithMinimumStats()).ToArray();
    
    private static WithMinimumStats GetNext()
    {
        _poolIndex = (_poolIndex + 1) % Pool.Length;
        return Pool[_poolIndex];
    }

    public static bool Init = true;
    
    public static IStats NotBelowZero(this IStats s, params StatType[] exceptions) => GetNext().Initialized(0, s, exceptions);
}
