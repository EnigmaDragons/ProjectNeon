using System.Collections.Generic;
using System.Linq;

public sealed class BattleStateSnapshot
{
    public List<CardTypeData[]> PlayedCardHistory { get; }
    public Dictionary<int, MemberSnapshot> Members { get; }

    public BattleStateSnapshot(params MemberSnapshot[] snapshots) 
        : this(new List<CardTypeData[]>(), snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(List<CardTypeData[]> playedCardHistory, params MemberSnapshot[] snapshots) 
        : this(playedCardHistory, snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(List<CardTypeData[]> playedCardHistory, Dictionary<int, MemberSnapshot> members)
    {
        PlayedCardHistory = playedCardHistory;
        Members = members;
    }

    public MemberSnapshot[] TargetMembers(Target target) => target.Members.Select(x => Members[x.Id]).ToArray();
}
