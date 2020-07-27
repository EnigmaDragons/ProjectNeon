using System.Collections.Generic;
using System.Linq;

public class Team : Target
{
    public Team(IEnumerable<Member> members)
    {
        Members = members.ToArray();
    }

    public Member[] Members { get; }
}
