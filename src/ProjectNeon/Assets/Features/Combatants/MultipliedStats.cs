using System.Collections.Generic;
using System.Linq;

public sealed class MultipliedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;

    public float this[StatType statType] => _first[statType] * _second[statType];
    
    // @todo #1:15min Combine MaxResource Amounts
    public IResourceType[] ResourceTypes => _first.ResourceTypes;

    public MultipliedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
    }
}

public static class MultipliedStatExtensions
{
    public static IStats Times(this IStats first, params IStats[] others) => Times(first, (IEnumerable<IStats>) others);
    public static IStats Times(this IStats first, IEnumerable<IStats> others) => others.Aggregate(first, (current, other) => new MultipliedStats(current, other));
}
