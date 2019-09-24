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
     * The current BattleState.
     */
    private BattleState battleState;

    public DumbAI Init (Member ai, BattleState state)
    {
        this.me = ai;
        this.battleState = state;
        return this;
    }

    public override PlayedCard Play()
    {
        Card chosen = me.GetDeck().Cards.Random();

        Target target = me;
        List<Member> team = new List<Member>();
        switch (chosen.group)
        {
            case Group.Enemy:
                team = this.battleState.GetEnemies();
                break;
            case Group.Ally:
                team = this.battleState.GetAllies();
                break;
            case Group.All:
                team = this.battleState.GetAllies();
                team.AddRange(this.battleState.GetEnemies());
                break;
        }
        switch (chosen.scope)
        {
            case Scope.One:
                target = team.Random();
                break;
            case Scope.All:
                target = new Team().Init(team);
                break;
            case Scope.Self:
                target = me;
                break;
        }

        return new PlayedCard().Init(me, target, chosen);
    }

}
