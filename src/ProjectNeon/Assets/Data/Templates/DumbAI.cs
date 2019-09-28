using System.Collections.Generic;
using UnityEngine;

public sealed class DumbAI : TurnAI
{
    [SerializeField] private BattleState battleState;

    public DumbAI Init(BattleState state)
    {
        battleState = state;
        return this;
    }

    public override PlayedCard Play(Enemy activeEnemy)
    {
        var me = activeEnemy.AsMember();
        var card = activeEnemy.Deck.Cards.Random();
        List<Target> targets = new List<Target>();
        card.Actions.ForEach(
            action =>
            {
                Target[] possibleTargets = battleState.GetPossibleTargets(me, action.Group, action.Scope);
                Target target = possibleTargets.Random();
                targets.Add(target);
            }
        );

        return new PlayedCard().Init(me, targets.ToArray(), card);
    }

}
