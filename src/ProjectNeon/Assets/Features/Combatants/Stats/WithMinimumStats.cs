using System.Linq;
using UnityEngine;

public sealed class WithMinimumStats : IStats
{
    private float _minimum;
    private IStats _inner;

    public float this[StatType statType] => Mathf.Max(_inner[statType], _minimum);
    public float this[TemporalStatType statType] => Mathf.Max(_inner[statType], _minimum);

    public IResourceType[] ResourceTypes => _inner.ResourceTypes;

    public WithMinimumStats() : this(0, new StatAddends()) {}
    public WithMinimumStats(float minimum, IStats inner) => Initialized(minimum, inner);

    public WithMinimumStats Initialized(float minimum, IStats inner)
    {
        _minimum = minimum;
        _inner = inner;
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
    
    public static IStats NotBelowZero(this IStats s) => GetNext().Initialized(0, s);
    public static IStats NotBelow(this IStats s, int amount) => GetNext().Initialized(amount, s);
}
