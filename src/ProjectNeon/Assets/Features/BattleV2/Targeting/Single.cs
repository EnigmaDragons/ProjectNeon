using System.Collections.Generic;

public class Single : Target
{
    private readonly Member _member;

    public Single(Member member) => _member = member;
    public Member[] Members => _member.AsArray();

    public override bool Equals(object obj) =>
        obj is Single single && EqualityComparer<Member>.Default.Equals(_member, single._member)
        || obj is Member member && EqualityComparer<Member>.Default.Equals(_member, member);

    public override string ToString() => $"{_member.Name} {_member.Id}";
    public override int GetHashCode() => -23316680 + EqualityComparer<Member>.Default.GetHashCode(_member);
}
