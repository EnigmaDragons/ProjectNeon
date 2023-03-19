using System;

public sealed class MemberSnapshot
{
    public int Id { get; }
    public string NameTerm { get; }
    public string ClassTerm { get; }
    public TeamType TeamType { get; }
    public MemberStateSnapshot State { get; }

    public MemberSnapshot(int memberId, string nameTerm, string memberClassTerm, TeamType teamType, MemberStateSnapshot stateSnapshot)
    {
        Id = memberId;
        NameTerm = nameTerm;
        ClassTerm = memberClassTerm;
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
    public static bool IsConscious(this MemberSnapshot m) => !IsUnconscious(m);
    public static bool IsBloodied(this MemberSnapshot m) => m.State.Hp <= m.State.MaxHp / 2;
    public static bool IsStealthed(this MemberSnapshot m) => m.State[TemporalStatType.Stealth] > 0;
}
