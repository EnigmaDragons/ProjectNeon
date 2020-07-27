
public sealed class MemberAsTarget : Target
{
    public TeamType TeamType => Members[0].TeamType;
    public Member[] Members { get; }

    public MemberAsTarget(Member member)
    {
        Members = member.AsArray();
    }
}
