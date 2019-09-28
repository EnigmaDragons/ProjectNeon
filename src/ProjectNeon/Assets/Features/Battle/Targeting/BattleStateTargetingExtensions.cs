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
        if (team == TeamType.Party)
            return state.GetPartyMembers();
        if (team == TeamType.Enemies)
            return state.GetEnemyMembers();
        return new Member[0];
    }

    public static Target[] GetPossibleEnemyTeamTargets(this BattleState state, Member self, Group group, Scope scope)
        => group == Group.Self
            ? new Target[] { new MemberAsTarget(self) }
            : NonSelfTargetsFor(state, TeamType.Enemies, group, scope);

    public static Target[] GetPossiblePlayerTargets(this BattleState state, Group group, Scope scope)
    {
        if (group == Group.Self)
            return new Target[0]; // Puzzle exists in SelectCardTargets.cs
        return NonSelfTargetsFor(state, TeamType.Party, group, scope);
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
