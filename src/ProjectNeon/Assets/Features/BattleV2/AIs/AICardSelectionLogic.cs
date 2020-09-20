using System;
using System.Collections.Generic;
using System.Linq;

public static class AICardSelectionLogic
{
    public static CardSelectionContext WithSelectedCardByNameIfPresent(this CardSelectionContext ctx, string cardName)
        => ctx.CardOptions.Any(c => c.Name.Equals(cardName))
            ? ctx.WithSelectedCard(ctx.CardOptions.First(c => c.Name.Equals(cardName)))
            : ctx;
    
    public static CardSelectionContext WithSelectedDesignatedAttackerCardIfApplicable(this CardSelectionContext ctx) 
        => ctx.SelectedCard.IsMissing && ctx.Strategy.DesignatedAttacker.Equals(ctx.Member) && ctx.CardOptions.Any(p => p.Is(CardTag.Attack))
            ? ctx.WithSelectedCard(ctx.SelectAttackCard())
            : ctx;

    public static CardSelectionContext WithSelectedUltimateIfAvailable(this CardSelectionContext ctx)
        => ctx.SelectedCard.IsMissing && ctx.CardOptions.Any(c => c.Tags.Contains(CardTag.Ultimate))
            ? ctx.WithSelectedCard(ctx.CardOptions.Where(c => c.Tags.Contains(CardTag.Ultimate)).MostExpensive())
            : ctx;

    public static CardSelectionContext WithCommonSenseSelections(this CardSelectionContext ctx)
        => ctx
            .DontPlayHealsIfAlliesDontNeedHealing()
            .DontPlayShieldsIfAlliesDontNeedShielding()
            .DontPlayShieldAttackIfOpponentsDontHaveManyShields()
            .DontRemoveResourcesIfOpponentsDontHaveMany();
    
    public static CardSelectionContext DontPlayShieldAttackIfOpponentsDontHaveManyShields(this CardSelectionContext ctx, int tolerance = 15)
        => ctx.IfTrueDontPlayType(x => x.Enemies.Sum(e => e.CurrentShield()) < tolerance, CardTag.Shield, CardTag.Attack)
            .IfTrueDontPlayType(x => x.Enemies.Sum(e => e.CurrentShield()) < tolerance, CardTag.RemoveShields);

    private static CardSelectionContext DontPlayHealsIfAlliesDontNeedHealing(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.CurrentHp() >= a.MaxHp() * 0.9), CardTag.Healing);

    private static CardSelectionContext DontPlayShieldsIfAlliesDontNeedShielding(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.RemainingShieldCapacity() > a.MaxShield() * 0.7), CardTag.Defense, CardTag.Shield);

    private static CardSelectionContext DontRemoveResourcesIfOpponentsDontHaveMany(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.State.PrimaryResourceValue < 1f), CardTag.RemoveResources);

    private static CardTypeData SelectAttackCard(this CardSelectionContext ctx) 
        => ctx.CardOptions.Where(o => o.Is(CardTag.Attack)).MostExpensive();

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx)
        => ctx.WithFinalizedCardSelection(_ => 0);
    
    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int> typePriority)
        => ctx.SelectedCard.IsMissing
            ? ctx.WithSelectedCard(FinalizeCardSelection(ctx, typePriority))
            : ctx;

    private static CardTypeData FinalizeCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int> typePriority) 
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

}
