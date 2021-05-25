using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/CanChain")]
public class CanChain : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => Evaluate(ctx) && ctx.BattleState.NumberOfCardPlaysRemainingThisTurn == 1;
    
    public static bool Evaluate(CardConditionContext ctx)
    {
        var card = ctx.Card;
        var battleState = ctx.BattleState;
        var hasChainedCard = card.ChainedCard.IsPresent;
        var partyMembersWhoHavePlayedCards = battleState
            .CurrentTurnCardPlays()
                .Where(x => x.Member.TeamType == TeamType.Party)
                .Select(m => m.Member.Id)
            .Concat(ctx.PendingCardsToResolve
                .Where(x => x.Member.TeamType == TeamType.Party)
                .Select(p => p.Member.Id));
        
        return hasChainedCard
               && partyMembersWhoHavePlayedCards.All(x => x.Equals(card.Owner.Id));
    }
}