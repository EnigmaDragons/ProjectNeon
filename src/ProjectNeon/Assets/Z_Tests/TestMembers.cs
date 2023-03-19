using System;
using System.Threading;
using UnityEngine;

public static class TestMembers
{
    private static bool _init = LookupInitializer.Init;
    
    private static int _id;
    private static int NextId() => Interlocked.Increment(ref _id);
    private static StatAddends DefaultStats() => new StatAddends()
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.MaxHP, 10f);

    public static Member Any() => Create(s => s.With(StatType.Damagability, 1));
    public static Member Named(string name) => Create(name, TeamType.Party, s => s.With(StatType.Damagability, 1));
    public static Member With(StatType statType, float value) => Create(s => s.With(statType, value));
    public static Member With(params IResourceType[] resources) =>  Create("Any Name", TeamType.Party, s => s.With(StatType.Damagability, 1), resources);
    
    public static Member Create(Func<StatAddends, StatAddends> initStats) => Create("Any Name", TeamType.Party, initStats);
    public static Member CreateEnemy(Func<StatAddends, StatAddends> initStats) => Create("Any Name", TeamType.Enemies, initStats);
    public static Member Create(string name, TeamType team, Func<StatAddends, StatAddends> initStats, params IResourceType[] resources)
    {
        var stats = initStats(DefaultStats().With(resources));
        return new Member(
            NextId(),
            name,
            "Any Class",
            MemberMaterialType.Organic,
            team,
            stats,
            BattleRole.Unknown,
            stats.DefaultPrimaryStat(),
            false,
            false
        );
    }

    public static Member AnyAlly() => Any();

    public static Member AnyEnemy()
    {
        var stats = DefaultStats();
        return new Member(NextId(), "Any Name", "Any Class", MemberMaterialType.Organic, TeamType.Enemies, stats, BattleRole.Unknown, stats.DefaultPrimaryStat(), false, false);
    }
}
