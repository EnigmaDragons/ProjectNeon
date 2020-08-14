using System.Collections.Generic;

public sealed class BattleStateSnapshot
{
    public Dictionary<int, MemberSnapshot> Members { get; }

    public BattleStateSnapshot(Dictionary<int, MemberSnapshot> members)
    {
        Members = members;
    }
}
