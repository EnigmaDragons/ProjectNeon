using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BattleStateSnapshot
{
    public BattleV2Phase Phase { get; }
    public List<PlayedCardSnapshot[]> PlayedCardHistory { get; }
    public Dictionary<int, MemberSnapshot> Members { get; }
    public int NumCardPlaysRemaining { get; }

    public Dictionary<int, MemberSnapshot> NonSelfAllies(int id)
    {
        var me = Members[id];
        return Members.Where(m => m.Value.TeamType == me.TeamType && m.Key != id).ToDictionary(k => k.Key, v => v.Value);
    }
    
    public Dictionary<int, MemberSnapshot> NonSelfConsciousAllies(int id)
    {
        var me = Members[id];
        return Members.Where(m => m.Value.IsConscious() && m.Value.TeamType == me.TeamType && m.Key != id).ToDictionary(k => k.Key, v => v.Value);
    }

    public BattleStateSnapshot(params MemberSnapshot[] snapshots) 
        : this(BattleV2Phase.NotBegun, new List<PlayedCardSnapshot[]>(), 0, snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(BattleV2Phase phase, List<PlayedCardSnapshot[]> playedCardHistory, int numCardPlaysRemaining, params MemberSnapshot[] snapshots) 
        : this(phase, playedCardHistory, numCardPlaysRemaining, snapshots.ToDictionary(m => m.Id, m => m)) {}
    
    public BattleStateSnapshot(BattleV2Phase phase, List<PlayedCardSnapshot[]> playedCardHistory,  int numCardPlaysRemaining, Dictionary<int, MemberSnapshot> members)
    {
        Phase = phase;
        PlayedCardHistory = playedCardHistory;
        Members = members;
        NumCardPlaysRemaining = numCardPlaysRemaining;
    }

    public MemberSnapshot[] TargetMembers(Target target) => target.Members.Select(x => Members[x.Id]).ToArray();
}
