using System;

public sealed class MemberSnapshot
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public MemberStateSnapshot State { get; }

    public MemberSnapshot(int memberId, string name, string memberClass, TeamType teamType, MemberStateSnapshot stateSnapshot)
    {
        Id = memberId;
        Name = name;
        Class = memberClass;
        TeamType = teamType;
        State = stateSnapshot;
    }

    public bool ChangedFrom(MemberSnapshot s)
    {
        if (s.Id != Id)
            throw new InvalidOperationException($"Cannot compare state of Member {Id} with state of Member {s.Id}");
        return State.VersionNumber == s.State.VersionNumber;
    }
}

public static class MemberSnapshotExtensions
{
    public static bool IsUnconscious(this MemberSnapshot m) => m.State.Hp < 1;
}
