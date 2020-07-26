using System.Linq;

public static class BattleStateTargetingExtensions
{
    private static Member[] GetConscious(this BattleState state, TeamType team)
    {
        return state.Members.Values.Where(x => x.TeamType == team && x.IsConscious()).ToArray();
    }

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
        var membersAsTargets = teamMembers.Select(x => new MemberAsTarget(x)).ToArray();

        return scope == Scope.One
            ? Targets(membersAsTargets)
            : Targets(new Team(teamMembers));
    }

    private static Target[] Targets(params Target[] targets) => targets;
}
