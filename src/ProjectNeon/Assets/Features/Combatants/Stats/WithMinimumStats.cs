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

    public InMemoryResourceType[] ResourceTypes => _inner.ResourceTypes;

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

public sealed class WithMinimumMaxHP : IStats
{
    private IStats _inner;

    public float this[StatType statType] => statType == StatType.MaxHP ? Mathf.Max(_inner[statType], 1) : _inner[statType];
    public float this[TemporalStatType statType] => _inner[statType];
    public InMemoryResourceType[] ResourceTypes => _inner.ResourceTypes;
    
    public WithMinimumMaxHP() : this(new StatAddends()) {}
    public WithMinimumMaxHP(IStats inner) => _inner = inner;
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
    public static IStats WithMaxHPMinimumOne(this IStats s) => new WithMinimumMaxHP(s);
}
