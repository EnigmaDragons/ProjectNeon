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

    public override IPlayedCard Play(int memberId)
    {
        var enemy = battleState.GetEnemyById(memberId);
        var me = battleState.Members[memberId];
        var card = enemy.Deck.Cards.Random();
        var targets = new List<Target>();
        card.Actions.ForEach(
            action =>
            {
                var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
                var target = possibleTargets.Random();
                targets.Add(target);
            }
        );

        return new PlayedCardV2(me, targets.ToArray(), card,  card.ResourcesSpent(me));
    }

}
