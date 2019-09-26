using System.Linq;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private EnemyArea enemies;
    
    // @todo #1:10min Replace this with Member once implemented.
    [SerializeField] private Member tempTarget;

    public Member[] GetAllies()
    {
        /**
         * @todo #55:30min We need references to Member Decks in BattleState so we can return all Members with their 
         *  correct state on GetAllies and GetEnemies methods. After we get these references, update GetAllies and 
         *  GetEnemies methods to return the correct Members
         */
        return new []
        {
            new Member().Init(party.characterOne), 
            new Member().Init(party.characterTwo), 
            new Member().Init(party.characterThree)
        };
    }

    public Member[] GetEnemies()
    {
        return enemies.Enemies.Select(x => new Member()).ToArray();
    }

    private Member[] Get(TeamType team)
    {
        if (team == TeamType.Party)
            return GetAllies();
        if (team == TeamType.Enemies)
            return GetEnemies();
        return new Member[0];
    }

    public Target[] GetPossibleEnemyTeamTargets(Member self, Group group, Scope scope) 
        => scope == Scope.Self 
            ? new Target[] { self } 
            : NonSelfTargetsFor(TeamType.Enemies, @group, scope);

    public Target[] GetPossiblePlayerTargets(Group group, Scope scope)
    {
        if (scope == Scope.Self)
            return new Target[0]; // Puzzle exists in SelectCardTargets.cs
        return NonSelfTargetsFor(TeamType.Party, group, scope);
    }

    private Target[] NonSelfTargetsFor(TeamType myTeam, Group group, Scope scope)
    {
        var opponentsAre = myTeam == TeamType.Party ? TeamType.Enemies : TeamType.Party;
        var teamMembers = group == Group.Ally ? Get(myTeam) : Get(opponentsAre);
        var membersAsTargets = teamMembers.Cast<Target>().ToArray();
        
        return scope == Scope.One 
            ? Targets(membersAsTargets) 
            : Targets(new Team(teamMembers));
    }

    private Target[] Targets(params Target[] targets) => targets;
}
