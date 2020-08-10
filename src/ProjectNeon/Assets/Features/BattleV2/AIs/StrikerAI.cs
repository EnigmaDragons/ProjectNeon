using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Striker")]
public sealed class StrikerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);

        var card = playableCards.MostExpensive();
        var targets = new List<Target>();
        
        card.ActionSequences.ForEach(
            action =>
            {
                var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
                targets.Add(possibleTargets.MostVulnerable());
            }
        );

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
