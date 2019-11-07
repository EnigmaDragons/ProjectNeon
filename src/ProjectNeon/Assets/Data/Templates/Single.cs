using System.Collections.Generic;

public class Single : Target
{
    private Member _member;

    public Single(Member member)
    {
        _member = member;
    }

    public Member[] Members => _member.AsArray();

    public override bool Equals(object obj)
    {
        return (obj is Single single &&
               EqualityComparer<Member>.Default.Equals(_member, single._member))
               ||
               (obj is Member member &&
               EqualityComparer<Member>.Default.Equals(_member, member))
               ;
    }

    public override int GetHashCode()
    {
        return -23316680 + EqualityComparer<Member>.Default.GetHashCode(_member);
    }
}