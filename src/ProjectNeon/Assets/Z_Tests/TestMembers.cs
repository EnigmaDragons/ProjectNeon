using System;
using System.Threading;

public static class TestMembers
{
    private static int _id;
    private static int NextId() => Interlocked.Increment(ref _id);
    private static StatAddends DefaultStats() => new StatAddends()
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.MaxHP, 10f);

    public static Member Any() => Create(s => s.With(StatType.Damagability, 1));
    public static Member Named(string name) => Create(name, s => s.With(StatType.Damagability, 1));
    public static Member With(StatType statType, float value) => Create(s => s.With(statType, value));

    public static Member Create(Func<StatAddends, StatAddends> initStats) => Create("Any Name", initStats);
    public static Member Create(string name, Func<StatAddends, StatAddends> initStats, params IResourceType[] resources) => 
        new Member(
            NextId(),
            name,
            "Any Class",
            TeamType.Party,
            initStats(DefaultStats().With(resources)),
            BattleRole.Unknown
        );
}
