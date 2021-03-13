using System.Collections.Generic;
using System.Linq;

public sealed class BattleStateSnapshot
{
    public BattleV2Phase Phase { get; }
    public List<CardTypeData[]> PlayedCardHistory { get; }
    public Dictionary<int, MemberSnapshot> Members { get; }

    public BattleStateSnapshot(params MemberSnapshot[] snapshots) 
        : this(BattleV2Phase.NotBegun, new List<CardTypeData[]>(), snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(BattleV2Phase phase, params MemberSnapshot[] snapshots) 
        : this(phase, new List<CardTypeData[]>(), snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(BattleV2Phase phase, List<CardTypeData[]> playedCardHistory, params MemberSnapshot[] snapshots) 
        : this(phase, playedCardHistory, snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(BattleV2Phase phase, List<CardTypeData[]> playedCardHistory, Dictionary<int, MemberSnapshot> members)
    {
        Phase = phase;
        PlayedCardHistory = playedCardHistory;
        Members = members;
    }

    public MemberSnapshot[] TargetMembers(Target target) => target.Members.Select(x => Members[x.Id]).ToArray();
}
