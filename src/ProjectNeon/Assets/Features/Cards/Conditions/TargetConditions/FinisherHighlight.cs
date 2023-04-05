using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Finisher")]
public class FinisherHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx) 
        => inversed != (Evaluate(ctx) && ctx.BattleState.NumberOfCardPlaysRemainingThisTurn == 1);

    public static bool Evaluate(TargetedCardConditionContext ctx)
    {
        var hasChainedCard = ctx.Card.ChainedCard.IsPresent;
        return hasChainedCard && Evaluate(ctx.Card.Owner.Id, ctx.BattleState.CurrentTurnCardPlays());
    }
    
    public static bool Evaluate(int memberId, PlayedCardSnapshot[] playedCardsThisTurn)
    {
        var cardOwnersInPlayOrder = playedCardsThisTurn
            .Where(x => x.Member.TeamType == TeamType.Party && x.CardSpeed != CardSpeed.Quick)
            .Select(x => x.Member.Id)
            .Reverse()
            .ToArray();

        return cardOwnersInPlayOrder.Length > 2 && cardOwnersInPlayOrder.Take(3).All(x => x.Equals(memberId));
    }
}
