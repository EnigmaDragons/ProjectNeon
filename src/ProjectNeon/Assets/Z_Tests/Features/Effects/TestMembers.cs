using System;
using System.Threading;

public static class TestMembers
{
    private static int _id;
    private static int NextId() => Interlocked.Increment(ref _id);

    public static Member Any() => Create(s => s);
    public static Member With(StatType statType, float value) => Create(s => s.With(statType, value));
    public static Member Create(Func<StatAddends, StatAddends> initStats) => 
        new Member(
            NextId(),
            "Any Name",
            "Any Class",
            TeamType.Party,
            initStats(new StatAddends())
        );
}
