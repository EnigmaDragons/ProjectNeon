using System.Linq;

public static class BattleStateTargetingExtensions
{
    private static TeamType EnemyTeamType(this Member m)
        => m.TeamType == TeamType.Enemies
            ? TeamType.Party
            : TeamType.Enemies;
    
    public static Member[] GetConsciousEnemies(this Member[] members, Member self)
        => members.GetConscious(self.EnemyTeamType()); 
    
    public static Member[] GetConsciousAllies(this Member[] members, Member self)
        => members.GetConscious(self.TeamType);
    
    public static Member[] GetConscious(this BattleState state, TeamType team)
        => state.MembersWithoutIds.GetConscious(team);
    
    public static Member[] GetConscious(this Member[] members, TeamType team) 
        => members.Where(x => x.TeamType == team && x.IsConscious()).ToArray();

    public static Member[] GetConscious(this Member[] members)
        => members.Where(x => x.IsConscious()).ToArray();
    
    public static Member[] GetConsciousAllies(this BattleState s, Member m)
        => s.MembersWithoutIds.GetConscious(m.TeamType);

    public static Member[] GetConsciousEnemies(this BattleState s, Member m)
        => s.MembersWithoutIds.GetConscious(m.EnemyTeamType());

    public static Target[] GetPossibleConsciousTargets(this BattleState s, Member self, Group group, Scope scope)
        => GetPossibleConsciousTargets(s.MembersWithoutIds, self, group, scope);
    
    public static Target[] GetPossibleConsciousTargets(this Member[] allMembers, Member self, Group group, Scope scope)
    {
        Target[] targets = new Target[0];
        switch (group)
        {
            case Group.Self:  {
                targets = new Target[] { new Single(self) };
                break;
            }
            case Group.All:  
            case Group.Opponent:
            case Group.Ally:
            {
                targets = NonSelfConsciousTargetsFor(allMembers, self, self.TeamType, group, scope);
                break;
            }
        }
        
        return targets;
    }

    private static Target[] NonSelfConsciousTargetsFor(this Member[] allMembers, Member originator, TeamType myTeam, Group group, Scope scope)
    {
        if (group == Group.All && scope == Scope.All)
            return Targets(new Multiple(allMembers));
        
        var opponentsAre = myTeam == TeamType.Party ? TeamType.Enemies : TeamType.Party;
        var teamMembers = group == Group.Ally 
            ? GetConscious(allMembers, myTeam) 
            : group == Group.Opponent 
                ? GetConscious(allMembers, opponentsAre) 
                : GetConscious(allMembers);

        if (scope == Scope.One)
        {
            var doesEnemyHaveTaunt = teamMembers.Any(m => m.HasTaunt() && m.TeamType != myTeam);
            var membersWithTaunt = teamMembers.Where(m => m.HasTaunt() || m.TeamType == myTeam).ToArray();
            var membersWithoutStealth = teamMembers.Where(m => !m.IsStealth() || m.TeamType == myTeam).ToArray();
            var members = doesEnemyHaveTaunt
                ? membersWithTaunt
                : membersWithoutStealth;
            var targets = members.Select(m => new Single(m)).Cast<Target>().ToArray();
            return Targets(originator.IsConfused() ? new[] { targets.Shuffled().First() } : targets);
        }

        if (scope == Scope.OneExceptSelf)
        {
            return NonSelfConsciousTargetsFor(allMembers, originator, myTeam, group, Scope.One)
                .Where(x => !x.Members.Contains(originator)).ToArray();
        }

        if (scope == Scope.AllExcept)
        {
            var targets = teamMembers
                .Select(exclusion => new Multiple(teamMembers.Where(x => x.Id != exclusion.Id).ToArray()))
                .Cast<Target>()
                .ToArray();
            return Targets(originator.IsConfused() ? new[] { targets.Shuffled().First() } : targets);   
        }

        if (scope == Scope.AllExceptSelf)
        {
            var targets = teamMembers
                .Select(exclusion => new Multiple(teamMembers.Where(x => x.Id != originator.Id).ToArray()))
                .Cast<Target>()
                .ToArray();
            return targets;
        }
        return Targets(new Multiple(teamMembers));
    }

    private static Target[] Targets(params Target[] targets) => targets;
}
