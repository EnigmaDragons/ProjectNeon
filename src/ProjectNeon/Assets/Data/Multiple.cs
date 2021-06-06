using System.Collections.Generic;
using System.Linq;

public class Multiple : Target
{
    public Member[] Members { get; }

    public Multiple(IEnumerable<Member> members)
        : this(members.ToArray()) {}
    
    public Multiple(Member[] members)
    {
        Members = members;
    }

    public override bool Equals(object obj) => obj is Multiple && obj.ToString() == Members.ToString();
    public override int GetHashCode() => ToString().GetHashCode();
    public override string ToString() => string.Join(",", Members.Select(m => m.Id.ToString()));
}
