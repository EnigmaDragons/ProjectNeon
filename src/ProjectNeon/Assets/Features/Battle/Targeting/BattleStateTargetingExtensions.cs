using System.Linq;

public static class BattleStateTargetingExtensions
{
    private static Member[] Get(this BattleState state, TeamType team)
    {
        return state.Members.Values.Where(x => x.TeamType == team).ToArray();
    }

    public static Target[] GetPossibleTargets(this BattleState state, Member self, Group group, Scope scope)
    {
        Target[] targets = new Target[0];
        switch (group)
        {
            case Group.Self:  {
                targets = new Target[] { new MemberAsTarget(self) };
                break;
            }
            case Group.All:  {
                targets = Targets(new Team(state.Members.Values));
                break;
            }
            case Group.Opponent:
            case Group.Ally:
            {
                targets = NonSelfTargetsFor(state, self.TeamType, group, scope);
                break;
            }
        }
        return targets;
    }


    private static Target[] NonSelfTargetsFor(this BattleState state, TeamType myTeam, Group group, Scope scope)
    {
            var opponentsAre = myTeam == TeamType.Party ? TeamType.Enemies : TeamType.Party;
            var teamMembers = group == Group.Ally ? Get(state, myTeam) : Get(state, opponentsAre);
            var membersAsTargets = teamMembers.Select(x => new MemberAsTarget(x)).ToArray();

            return scope == Scope.One
                ? Targets(membersAsTargets)
                : Targets(new Team(teamMembers));
    }

    private static Target[] Targets(params Target[] targets) => targets;
}
