using System.Collections.Generic;
using System.Linq;

public sealed class BattleStateSnapshot
{
    public Dictionary<int, MemberSnapshot> Members { get; }

    public BattleStateSnapshot(params MemberSnapshot[] snapshots) 
        : this(snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(Dictionary<int, MemberSnapshot> members)
    {
        Members = members;
    }
}
