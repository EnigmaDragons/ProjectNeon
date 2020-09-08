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

        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithSelectedUltimateIfAvailable();
        
        IEnumerable<CardTypeData> cardOptions = playableCards;
        // Don't buff self if already buffed
        if (me.HasAttackBuff())
            cardOptions = cardOptions.Where(c => !c.Tags.Contains(CardTag.BuffAttack));

        var card = ctx.WithOptions(cardOptions)
            .FinalizeCardSelection();
        
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
