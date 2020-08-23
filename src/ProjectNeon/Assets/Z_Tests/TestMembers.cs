using System;
using System.Threading;

public static class TestMembers
{
    private static int _id;
    private static int NextId() => Interlocked.Increment(ref _id);
    private static StatAddends DefaultStats() => new StatAddends().With(StatType.Damagability, 1f).With(StatType.MaxHP, 10f);

    public static Member Any() => Create(s => s.With(StatType.Damagability, 1));
    public static Member With(StatType statType, float value) => Create(s => s.With(statType, value));
    public static Member Create(Func<StatAddends, StatAddends> initStats) => 
        new Member(
            NextId(),
            "Any Name",
            "Any Class",
            TeamType.Party,
            initStats(DefaultStats()),
            BattleRole.Unknown
        );
    
    public static Member Create(Func<StatAddends, StatAddends> initStats, params IResourceType[] resources) => 
        new Member(
            NextId(),
            "Any Name",
            "Any Class",
            TeamType.Party,
            initStats(DefaultStats().With(resources)),
            BattleRole.Unknown
        );
}
