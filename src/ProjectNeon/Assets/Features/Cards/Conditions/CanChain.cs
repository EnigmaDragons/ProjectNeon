using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/CanChain")]
public class CanChain : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => Evaluate(ctx) && ctx.BattleState.NumberOfCardPlaysRemainingThisTurn == 1;
    
    public static bool Evaluate(CardConditionContext ctx)
    {
        var hasChainedCard = ctx.Card.ChainedCard.IsPresent;
        return hasChainedCard && Evaluate(ctx.Card.Owner.Id, ctx.PendingCardsToResolve.ToArray(), ctx.BattleState.CurrentTurnCardPlays());
    }
    
    public static bool Evaluate(int memberId, IPlayedCard[] pendingCards, PlayedCardSnapshot[] playedCardsThisTurn)
    {
        var partyMembersWhoHavePlayedCards = 
            playedCardsThisTurn
                .Where(x => x.Member.TeamType == TeamType.Party && x.Card.Speed != CardSpeed.Quick)
                .Select(x => x.Member.Id)
            .Concat(pendingCards
                .Where(x => x.Member.TeamType == TeamType.Party && x.Card.Speed != CardSpeed.Quick)
                .Select(p => p.Member.Id))
            .ToList();

        return partyMembersWhoHavePlayedCards.Any() && partyMembersWhoHavePlayedCards.All(x => x.Equals(memberId));
    }
}
