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
}
