using System;
using System.Linq;

public static class AITargetSelectionLogic
{
    public static Target[] SelectedTargets(this CardSelectionContext ctx)
    {
        if (ctx.SelectedCard.IsMissing)
            throw new InvalidOperationException("Cannot select targets before a card is selected");

        var card = ctx.SelectedCard.Value;
        return card.ActionSequences.Select(action =>
        {
            var possibleTargets = ctx.State.GetPossibleConsciousTargets(ctx.Member, action.Group, action.Scope);
            if (card.Is(CardTag.Stun))
                return possibleTargets.MostPowerful();
            if (card.Is(CardTag.Attack))
                return ctx.Strategy.AttackTargetFor(action);
            if (card.Is(CardTag.Healing))
                return possibleTargets.MostDamaged();
            if (card.Is(CardTag.Defense, CardTag.Shield))
            {
                if (possibleTargets.Any(x => !x.HasShield()))
                    return possibleTargets.Where(x => !x.HasShield())
                        .MostVulnerable();
                // Or, use shield to whomever could use the most
                return possibleTargets.OrderByDescending(x => x.TotalRemainingShieldCapacity()).First();
            }
            return possibleTargets.Random();
        }).ToArray();

    }

    public static PlayedCardV2 WithSelectedTargetsPlayedCard(this CardSelectionContext ctx)
    {
        var targets = ctx.SelectedTargets();
        var card = ctx.SelectedCard.Value.CreateInstance(ctx.State.GetNextCardId(), ctx.Member);
        return new PlayedCardV2(ctx.Member, targets, card);
    }
}
