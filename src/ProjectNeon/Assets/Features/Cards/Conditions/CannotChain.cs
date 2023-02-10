using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/CannotChain")]
public class CannotChain : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => Evaluate(ctx) && ctx.BattleState.NumberOfCardPlaysRemainingThisTurn != 1;
    
    public override string Description => "This will not chain";
    
    public static bool Evaluate(CardConditionContext ctx)
    {
        var hasChainedCard = ctx.Card.ChainedCard.IsPresent;
        return !hasChainedCard || Evaluate(ctx.Card.Owner.Id, ctx.PendingCardsToResolve.ToArray(), ctx.BattleState.CurrentTurnCardPlays());
    }
    
    public static bool Evaluate(int memberId, IPlayedCard[] pendingCards, PlayedCardSnapshot[] playedCardsThisTurn)
    {
        var cardOwnersInPlayOrder = playedCardsThisTurn
            .Where(x => x.Member.TeamType == TeamType.Party && x.Card.Speed != CardSpeed.Quick)
            .Select(x => x.Member.Id)
            .Concat(pendingCards
                .Where(x => x.Member.TeamType == TeamType.Party && x.Card.Speed != CardSpeed.Quick)
                .Select(x => x.Member.Id))
            .Reverse()
            .ToArray();

        return cardOwnersInPlayOrder.Length < 2 || cardOwnersInPlayOrder.Take(3).Any(x => !x.Equals(memberId));
    }
}