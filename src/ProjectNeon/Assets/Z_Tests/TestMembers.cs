using System;
using System.Threading;

public static class TestMembers
{
    private static int _id;
    private static int NextId() => Interlocked.Increment(ref _id);

    public static Member Any() => Create(s => s.With(StatType.Damagability, 1));
    public static Member With(StatType statType, float value) => Create(s => s.With(StatType.Damagability, 1f).With(statType, value));
    public static Member Create(Func<StatAddends, StatAddends> initStats) => 
        new Member(
            NextId(),
            "Any Name",
            "Any Class",
            TeamType.Party,
            initStats(new StatAddends().With(StatType.Damagability, 1f))
        );
    public static Member Create(Func<StatAddends, StatAddends> initStats, params IResourceType[] resources) => 
        new Member(
            NextId(),
            "Any Name",
            "Any Class",
            TeamType.Party,
            initStats(new StatAddends().With(StatType.Damagability, 1f).With(resources))
        );
}
