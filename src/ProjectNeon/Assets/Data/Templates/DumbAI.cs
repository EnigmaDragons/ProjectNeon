using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        var playableCards = enemy.Deck.Cards.Where(c => c.IsPlayableBy(me)).ToList();
        if (!playableCards.Any())
            throw new InvalidDataException($"{me.Name} {me.Id} has no playable cards in hand");
        var card = playableCards.Random();
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
