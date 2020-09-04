using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        
        var card = playableCards
            .ToArray()
            .Shuffled()
            .OrderByDescending(c => c.Cost.Amount)
            .First();
        
        var targets = card.ActionSequences.Select(action => 
        {
            var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
            if (card.Is(CardTag.Healing))
                return possibleTargets.MostDamaged();
            if (card.Is(CardTag.Defense, CardTag.Shield))
            {
                if (possibleTargets.Any(x => !x.HasShield()))
                    return possibleTargets.Where(x => !x.HasShield()).MostVulnerable();
                // Or, use shield to whomever could use the most
                return possibleTargets.OrderByDescending(x => x.TotalRemainingShieldCapacity()).First();
            }

            if (card.Is(CardTag.Attack))
                return strategy.AttackTargetFor(action);
            return possibleTargets.Random();
        });
        
        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
