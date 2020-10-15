using System;
using System.Collections.Generic;
using System.Linq;

public static class AITargetSelectionLogic
{
    public static Target[] SelectedTargets(this CardSelectionContext ctx)
        => SelectedTargets(ctx, _ => true);
    
    public static Target[] SelectedTargets(this CardSelectionContext ctx, Func<Target, bool> isPreferredTarget)
    {
        if (ctx.SelectedCard.IsMissing)
            throw new InvalidOperationException("Cannot select targets before a card is selected");

        var card = ctx.SelectedCard.Value;
        return card.ActionSequences.Select(action =>
        {
            var possibleTargets = ctx.State.GetPossibleConsciousTargets(ctx.Member, action.Group, action.Scope);
            if (possibleTargets.Where(isPreferredTarget).Any())
                possibleTargets = possibleTargets.Where(isPreferredTarget).ToArray();
            
            if (card.Is(CardTag.Stun))
            {
                var stunnedTargets = possibleTargets.Where(p => p.Members.Any(m => m.IsStunnedForCurrentTurn()));
                var stunSelectedTargets = ctx.Strategy.SelectedNonStackingTargets.TryGetValue(CardTag.Stun, out var targets) ? targets : new HashSet<Target>();
                var saneTargets = possibleTargets.Except(stunSelectedTargets).Except(stunnedTargets);
                var actualTargets = saneTargets.Any() ? saneTargets : possibleTargets;
                return actualTargets.MostPowerful();
            }

            if (card.Is(CardTag.BuffResource) && action.Group == Group.Ally)
                return possibleTargets.MostPowerful();
            if (card.Is(CardTag.BuffAttack) && action.Group == Group.Ally)
                return possibleTargets.BestAttackerToBuff(ctx.Strategy);
            if (card.Is(CardTag.RemoveResources) && action.Group == Group.Opponent)
                return possibleTargets.MostResources();
            if ((card.Is(CardTag.RemoveShields) || card.Is(CardTag.Attack, CardTag.Shield)) && action.Group == Group.Opponent)
                return possibleTargets.MostShielded();
            if (card.Is(CardTag.Blind))
                return possibleTargets.MostAttack();
            if (card.Is(CardTag.Vulnerable) && action.Group == Group.Opponent)
            {
                var vulnerableTargets = possibleTargets.Where(p => p.Members.Any(m => m.IsVulnerable()));
                var vulnerableSelectedTargets = ctx.Strategy.SelectedNonStackingTargets.TryGetValue(CardTag.Vulnerable, out var targets) ? targets : new HashSet<Target>();
                var saneTargets = possibleTargets.Except(vulnerableSelectedTargets).Except(vulnerableTargets).ToArray();
                return saneTargets.Any(x => x == ctx.Strategy.AttackTargetFor(action)) 
                    ? ctx.Strategy.AttackTargetFor(action) 
                    : saneTargets.Any() 
                        ? saneTargets.Random() 
                        : possibleTargets.Random();
            }
            if (card.Is(CardTag.Attack) && action.Group == Group.Opponent)
                return Rng.Chance(0.80) ? ctx.Strategy.AttackTargetFor(action) : possibleTargets.Random();
            if (card.Is(CardTag.Healing) && action.Group == Group.Ally)
                return possibleTargets.MostDamaged();
            if (card.Is(CardTag.Defense, CardTag.Shield) && action.Group == Group.Ally)
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
        => WithSelectedTargetsPlayedCard(ctx, _ => true);
    
    public static PlayedCardV2 WithSelectedTargetsPlayedCard(this CardSelectionContext ctx, Func<Target, bool> isPreferredTarget)
    {
        if (ctx.SelectedCard.IsMissing)
            ctx = ctx.WithFinalizedCardSelection();
        
        BattleLog.Write($"{ctx.Member.Name} choose {ctx.SelectedCard.Value.Name} out of [{string.Join(", ", ctx.CardOptions.Select(x => x.Name))}]");
        
        var targets = ctx.SelectedTargets(isPreferredTarget);
        
        var card = ctx.SelectedCard.Value.CreateInstance(ctx.State.GetNextCardId(), ctx.Member);
        RecordNonStackingTags(CardTag.Stun, ctx, targets);
        RecordNonStackingTags(CardTag.Vulnerable, ctx, targets);
        RecordNonStackingTags(CardTag.Taunt, ctx, targets);
        
        return new PlayedCardV2(ctx.Member, targets, card);
    }

    private static void RecordNonStackingTags(CardTag tag, CardSelectionContext ctx, Target[] targets)
    {
        if (ctx.SelectedCard.Value.Is(tag))
            targets.ForEach(t => ctx.Strategy.RecordNonStackingTarget(tag, t));
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
    
    public static Target MostAttack(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(t => t.TotalAttack())
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
    
    public static Target MostShielded(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.TotalShields())
        .First();

    public static Target MostResources(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.TotalResourceValue())
        .First();
}
