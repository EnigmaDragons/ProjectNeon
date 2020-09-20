using System.Linq;

public static class BattleStateTargetingExtensions
{
    public static Member[] GetConsciousAllies(this BattleState s, Member m)
        => s.GetConscious(m.TeamType);

    public static Member[] GetConsciousEnemies(this BattleState s, Member m)
        => s.GetConscious(m.TeamType == TeamType.Enemies 
            ? TeamType.Party 
            : TeamType.Enemies); 
    
    public static Member[] GetConscious(this BattleState state, TeamType team) 
        => state.Members.Values.Where(x => x.TeamType == team && x.IsConscious()).ToArray();

    public static Target[] GetPossibleConsciousTargets(this BattleState state, Member self, Group group, Scope scope)
    {
        Target[] targets = new Target[0];
        switch (group)
        {
            case Group.Self:  {
                targets = new Target[] { new MemberAsTarget(self) };
                break;
            }
            case Group.All:  {
                targets = Targets(new Team(state.Members.Values.Where(m => m.IsConscious())));
                break;
            }
            case Group.Opponent:
            case Group.Ally:
            {
                targets = NonSelfConsciousTargetsFor(state, self.TeamType, group, scope);
                break;
            }
        }
        return targets;
    }

    private static Target[] NonSelfConsciousTargetsFor(this BattleState state, TeamType myTeam, Group group, Scope scope)
    {
        var opponentsAre = myTeam == TeamType.Party ? TeamType.Enemies : TeamType.Party;
        var teamMembers = group == Group.Ally ? GetConscious(state, myTeam) : GetConscious(state, opponentsAre);

        if (scope == Scope.One)
        {
            var membersWithTaunt = teamMembers.Where(m => m.HasTaunt()).ToArray();
            var membersWithoutStealth = teamMembers.Where(m => !m.IsStealth()).ToArray();
            var members = membersWithTaunt.Any() 
                ? membersWithTaunt 
                : membersWithoutStealth.Any() 
                    ? membersWithoutStealth 
                    : teamMembers;
            return Targets(members.Select(m => new Single(m)).Cast<Target>().ToArray());
        }

        if (scope == Scope.AllExcept)
            return Targets(teamMembers.Select(exclusion => new Multiple(teamMembers.Where(x => x != exclusion).ToArray())).Cast<Target>().ToArray());
        return Targets(new Team(teamMembers));
    }

    private static Target[] Targets(params Target[] targets) => targets;
}
