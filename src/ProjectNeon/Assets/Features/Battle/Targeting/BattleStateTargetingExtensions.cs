using System.Linq;

public static class BattleStateTargetingExtensions
{
    // @todo #1:30min Needs to handle Group All scenarios.

    public static Member[] GetPartyMembers(this BattleState state)
    {
        return new[]
        {
            new Member(TeamType.Party, state.Party.heroOne.Stats),
            new Member(TeamType.Party, state.Party.heroTwo.Stats),
            new Member(TeamType.Party, state.Party.heroThree.Stats)
        };
    }

    public static Member[] GetEnemyMembers(this BattleState state)
    {
        return state.EnemyArea.Enemies.Select(x => x.AsMember()).ToArray();
    }

    private static Member[] Get(this BattleState state, TeamType team)
    {
        return state.Members.Values.Where(x => x.TeamType == team).ToArray();
    }

    public static Target[] GetPossibleTargets(this BattleState state, Member self, Group group, Scope scope)
        => group == Group.Self
            ? new Target[] { new MemberAsTarget(self) }
            : NonSelfTargetsFor(state, self.TeamType, group, scope);

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
