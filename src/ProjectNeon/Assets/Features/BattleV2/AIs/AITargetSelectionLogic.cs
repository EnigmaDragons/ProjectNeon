using System;
using System.Collections.Generic;
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
            {
                var stunnedTargets = possibleTargets.Where(p => p.Members.Any(m => m.IsStunnedForCurrentTurn()));
                var stunSelectedTargets = ctx.Strategy.SelectedNonStackingTargets.TryGetValue(CardTag.Stun, out var targets) ? targets : new HashSet<Target>();
                var saneTargets = possibleTargets.Except(stunSelectedTargets).Except(stunnedTargets);
                var actualTargets = saneTargets.Any() ? saneTargets : possibleTargets;
                return actualTargets.MostPowerful();
            }

            if (card.Is(CardTag.BuffResource))
                return possibleTargets.MostPowerful();
            if (card.Is(CardTag.BuffAttack))
                return possibleTargets.BestAttackerToBuff(ctx.Strategy); 
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
        if (ctx.SelectedCard.Value.Is(CardTag.Stun))
            targets.ForEach(t => ctx.Strategy.RecordNonStackingTarget(CardTag.Stun, t));
        
        return new PlayedCardV2(ctx.Member, targets, card);
    }
    
    // TODO: In the future, factor in armor/resist/vulnerable
    public static Target MostVulnerable(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderBy(t => t.TotalHpAndShields())
        .First();

    public static Target MostPowerful(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(t => t.TotalOffense() * 50 + t.TotalHpAndShields() * 20)
        .First();

    public static Target MostDamaged(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.TotalMissingHp())
        .First();
    
    public static Target BestAttackerToBuff(this IEnumerable<Target> targets, AIStrategy strategy) =>
        targets
            .ToArray()
            .Shuffled()
            .OrderByDescending(p =>
                (p.Members.Contains(strategy.DesignatedAttacker) ? 1 : 0) * 100 // Prefer Designated Attack for immediate power
                + p.Members.Count(x => x.BattleRole == BattleRole.Striker) * 50  // Prefer Strikers
                + p.TotalAttack()) // Prefer more effective
            .First();
}
