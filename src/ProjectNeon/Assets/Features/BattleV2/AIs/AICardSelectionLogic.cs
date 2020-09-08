using System;
using System.Collections.Generic;
using System.Linq;

public static class AICardSelectionLogic
{
    public static CardSelectionContext WithSelectedDesignatedAttackerCardIfApplicable(this CardSelectionContext ctx) 
        => ctx.SelectedCard.IsMissing && ctx.Strategy.DesignatedAttacker.Equals(ctx.Member) && ctx.CardOptions.Any(p => p.Is(CardTag.Attack))
            ? ctx.withSelectedCard(ctx.SelectAttackCard())
            : ctx;

    public static CardSelectionContext WithSelectedUltimateIfAvailable(this CardSelectionContext ctx)
        => ctx.SelectedCard.IsMissing && ctx.CardOptions.Any(c => c.Tags.Contains(CardTag.Ultimate))
            ? ctx.withSelectedCard(ctx.CardOptions.Where(c => c.Tags.Contains(CardTag.Ultimate)).MostExpensive())
            : ctx;

    public static CardTypeData SelectAttackCard(this CardSelectionContext ctx) 
        => ctx.CardOptions.Where(o => o.Is(CardTag.Attack)).MostExpensive();

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx)
        => ctx.WithFinalizedCardSelection(_ => 0);
    
    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int> typePriority)
        => ctx.SelectedCard.IsMissing
            ? ctx.withSelectedCard(FinalizeCardSelection(ctx, typePriority))
            : ctx;

    public static CardTypeData FinalizeCardSelection(this CardSelectionContext ctx)
        => ctx.FinalizeCardSelection(_ => 0);
    
    public static CardTypeData FinalizeCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int> typePriority) 
        => ctx.SelectedCard.IsPresent 
            ? ctx.SelectedCard.Value
            : ctx.CardOptions
                .ToArray()
                .Shuffled()
                .OrderByDescending(c => c.Cost.Amount)
                .ThenBy(typePriority)
                .First();
    
    public static CardTypeData MostExpensive(this IEnumerable<CardTypeData> cards) => cards
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.Cost.Amount)
        .First();

    // TODO: In the future, factor in armor/resist/vulnerable
    public static Target MostVulnerable(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderBy(t => t.TotalHpAndShields())
        .First();

    public static Target MostPowerful(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(t => t.TotalOffense())
        .First();

    public static Target MostDamaged(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.TotalMissingHp())
        .First();
}
