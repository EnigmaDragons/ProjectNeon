using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Striker")]
public sealed class StrikerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);

        IEnumerable<CardType> cardOptions = playableCards;
        Maybe<CardType> maybeCard = Maybe<CardType>.Missing();
        
        // Always play a Super Card if charged
        if (me.HasMaxPrimaryResource())
            maybeCard = playableCards.MostExpensive();
        
        // Don't buff self if already buffed
        if (me.HasAttackBuff())
            cardOptions = cardOptions.Where(c => !c.Tags.Contains(CardTag.BuffAttack));

        var card = maybeCard.IsPresent
            ? maybeCard.Value
            : cardOptions.MostExpensive();
        
        var targets = new List<Target>();
        
        card.ActionSequences.ForEach(
            action =>
            {
                if (action.Group == Group.Opponent)
                    targets.Add(strategy.AttackTargetFor(action));
                else
                {
                    var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);   
                    targets.Add(possibleTargets.MostVulnerable());
                }
            }
        );

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
