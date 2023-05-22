using System.Linq;

public sealed class WithPowerCountingAsStat : IStats
{
    private IStats _inner;
    private IStats _baseStats;
    private StatType _primary;

    public WithPowerCountingAsStat() : this(new StatAddends(), new StatAddends(), StatType.Attack) {}
    private WithPowerCountingAsStat(IStats inner,  IStats baseStats, StatType primary) => Initialized(inner, baseStats, primary);

    public WithPowerCountingAsStat Initialized(IStats inner, IStats baseStats, StatType primary)
    {
        _inner = inner;
        _primary = primary;
        _baseStats = baseStats ?? new StatAddends();
        return this;
    }

    public float this[StatType statType] => statType == StatType.Power
        ? 0
        : statType == _primary
            ? _inner[statType] + (_inner[StatType.Power] - _baseStats[StatType.Power]) // Only add Additional Power
            : _inner[statType];

    public float this[TemporalStatType statType] => _inner[statType];
    public InMemoryResourceType[] ResourceTypes => _inner.ResourceTypes;
}

public static class WithPowerCountingAsStatExtensions
{
    private static int _poolIndex = 0;
    private static readonly WithPowerCountingAsStat[] Pool = Enumerable.Range(0, 500).Select(_ => new WithPowerCountingAsStat()).ToArray();
    
    private static WithPowerCountingAsStat GetNext()
    {
        _poolIndex = (_poolIndex + 1) % Pool.Length;
        return Pool[_poolIndex];
    }

    public static bool Init = true;
    
    public static IStats WithPowerCountedAs(this IStats s, IStats baseStats, StatType primary) => GetNext().Initialized(s, baseStats, primary);
}