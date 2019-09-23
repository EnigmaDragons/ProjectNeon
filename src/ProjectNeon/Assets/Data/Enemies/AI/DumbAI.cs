using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbAI : TurnAI
{
    /**
     * The AI member
     */
    private Member me;

    /**
     * This AI allied members.
     */
    private List<Member> allies;

    /**
     * This AI enemy members.
     */
    private List<Member> enemies;

    public override PlayedCard play()
    {
        Card chosen = me.GetDeck().GetCards().Random();

        Target target = me;
        List<Member> team = new List<Member>();
        switch (chosen.group)
        {
            case Group.Enemy:
                team = enemies;
                break;
            case Group.Ally:
                team = allies;
                break;
            case Group.All:
                team = allies;
                team.AddRange(enemies);
                break;
        }
        switch (chosen.scope)
        {
            case Scope.One:
                target = team.Random();
                break;
            case Scope.All:
                target = new Team(team);
                break;
            case Scope.Self:
                target = me;
                break;
        }

        return new PlayedCard(me, target, chosen);
    }

}
