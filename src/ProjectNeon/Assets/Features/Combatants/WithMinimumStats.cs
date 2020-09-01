
using UnityEngine;

public sealed class WithMinimumStats : IStats
{
    private readonly float _minimum;
    private readonly IStats _inner;

    public float this[StatType statType] => Mathf.Max(_inner[statType], _minimum);
    public float this[TemporalStatType statType] => Mathf.Max(_inner[statType], _minimum);

    public IResourceType[] ResourceTypes => _inner.ResourceTypes;
    
    public WithMinimumStats(float minimum, IStats inner)
    {
        _minimum = minimum;
        _inner = inner;
    }
}

public static class WithMinimumStatsExtensions
{
    public static IStats NotBelow(this IStats s, int amount) => new WithMinimumStats(amount, s);
}
