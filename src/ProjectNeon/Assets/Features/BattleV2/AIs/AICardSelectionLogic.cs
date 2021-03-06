using System;
using System.Collections.Generic;
using System.Linq;

public static class AICardSelectionLogic
{
    public static CardSelectionContext WithSelectedCardByNameIfPresent(this CardSelectionContext ctx, string cardName)
        => ctx.SelectedCard.IsMissing && ctx.CardOptions.Any(c => c.Name.Equals(cardName))
            ? ctx.WithSelectedCard(ctx.CardOptions.First(c => c.Name.Equals(cardName)))
            : ctx;

    public static CardSelectionContext WithRemovedCardByNameIfPresent(this CardSelectionContext ctx, string cardName)
        => ctx.WithCardOptions(ctx.CardOptions.Where(x => !x.Name.Equals(cardName)));
    
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
            .DontPlaySelfAttackBuffIfAlreadyBuffed()
            .DontPlayShieldAttackIfOpponentsDontHaveManyShields()
            .DontRemoveResourcesIfOpponentsDontHaveMany()
            .DontPlayTauntIfAnyAllyIsPlayingOne()
            .DontPlayMagicalCountersIfOpponentsAreNotMagical()
            .DontPlayPhysicalCountersIfOpponentsAreNotPhysical()
            .DontGiveExtraResourcesIfAlliesHaveEnough()
            .DontPlayXCostWithZeroResources()
            .DontPlayHealsIfAlliesDontNeedHealing()
            .DontPlayShieldsIfAlliesDontNeedShielding()
            .DontGiveAlliesDodgeIfTheyAlreadyHaveEnough()
            .DontGiveAlliesAegisIfTheyAlreadyHaveEnough()
            .DontStealCreditsIfOpponentDoesntHaveAny();

    public static CardSelectionContext DontGiveAlliesDodgeIfTheyAlreadyHaveEnough(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlay(x => x.Allies.Sum(a => a.State[TemporalStatType.Dodge]) >= x.Allies.Length, c => c.Is(CardTag.Defense, CardTag.Dodge));
    
    public static CardSelectionContext DontGiveAlliesAegisIfTheyAlreadyHaveEnough(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlay(x => x.Allies.Sum(a => a.State[TemporalStatType.Aegis]) >= x.Allies.Length, c => c.Is(CardTag.Defense, CardTag.Aegis));
    
    public static CardSelectionContext DontPlaySelfAttackBuffIfAlreadyBuffed(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlay(x => x.Member.Attack() > x.Member.State.BaseStats.Attack(), c => c.Is(CardTag.BuffAttack, CardTag.SelfOnly));
    
    public static CardSelectionContext DontPlayXCostWithZeroResources(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlay(x => x.Member.PrimaryResourceAmount() == 0, c => c.Cost.PlusXCost);
    
    public static CardSelectionContext DontGiveExtraResourcesIfAlliesHaveEnough(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.Except(ctx.Member).All(e => e.RemainingPrimaryResourceCapacity() < 2), CardTag.BuffResource);
    
    public static CardSelectionContext DontPlayPhysicalCountersIfOpponentsAreNotPhysical(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.Attack() == 0), CardTag.DebuffPhysical)
            .IfTrueDontPlayType(x => x.Enemies.All(e => e.Attack() == 0), CardTag.Armor);

    public static CardSelectionContext DontPlayMagicalCountersIfOpponentsAreNotMagical(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.Magic() == 0), CardTag.DebuffMagical)
            .IfTrueDontPlayType(x => x.Enemies.All(e => e.Magic() == 0), CardTag.Resistance);
            
    public static CardSelectionContext DontPlayTauntIfAnyAllyIsPlayingOne(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Strategy.SelectedNonStackingTargets.ContainsKey(CardTag.Taunt), CardTag.Taunt);
    
    public static CardSelectionContext DontPlayShieldAttackIfOpponentsDontHaveManyShields(this CardSelectionContext ctx, int minShields = 15)
        => ctx.IfTrueDontPlayType(x => x.Enemies.Sum(e => e.CurrentShield()) < minShields, CardTag.Shield, CardTag.Attack)
            .IfTrueDontPlayType(x => x.Enemies.Sum(e => e.CurrentShield()) < minShields, CardTag.RemoveShields);

    private static CardSelectionContext DontPlayHealsIfAlliesDontNeedHealing(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.CurrentHp() >= a.MaxHp() * 0.9), CardTag.Healing);

    private static CardSelectionContext DontPlayShieldsIfAlliesDontNeedShielding(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.CurrentShield() > a.MaxShield() * 0.7), CardTag.Defense, CardTag.Shield);

    private static CardSelectionContext DontRemoveResourcesIfOpponentsDontHaveMany(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.State.PrimaryResourceValue < 1f), CardTag.RemoveResources);

    private static CardSelectionContext DontStealCreditsIfOpponentDoesntHaveAny(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.PartyAdventureState.Credits <= 0, CardTag.StealCredits);

    private static CardTypeData SelectAttackCard(this CardSelectionContext ctx) 
        => ctx.CardOptions.Where(o => o.Is(CardTag.Attack)).MostExpensive();

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx)
        => ctx.WithFinalizedCardSelection(_ => 0);

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, params CardTag[] tagPriority)
    {
        var dictionary = new DictionaryWithDefault<CardTag, int>(99);
        for (int i = 1; i < tagPriority.Length + 1; i++)
            dictionary[tagPriority[i - 1]] = i;
        return ctx.WithFinalizedCardSelection(c => dictionary[c.Tags.First()]);
    }
    
    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int> typePriority)
        => ctx.SelectedCard.IsMissing
            ? ctx.WithSelectedCard(FinalizeCardSelection(ctx, typePriority))
            : ctx;
    
    public static CardSelectionContext WithPhases(this CardSelectionContext ctx)
    {
        var phase = ctx.Member.State[TemporalStatType.Phase];
        var phaselessCards = ctx.CardOptions.Where(x => !x.Tags.Contains(CardTag.Phase1) && !x.Tags.Contains(CardTag.Phase2) && !x.Tags.Contains(CardTag.Phase3));
        if (phase == 1)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase1))));
        if (phase == 2)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase2))));
        if (phase == 3)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase3))));
        return ctx;
    }

    private static CardTypeData FinalizeCardSelection(this CardSelectionContext ctx,
        Func<CardTypeData, int> typePriority)
    {
        var card = ctx.SelectedCard.IsPresent 
            ? ctx.SelectedCard.Value
            : ctx.CardOptions.Any()
                ? ctx.CardOptions
                    .ToArray()
                    .Shuffled()
                    .OrderByDescending(c => c.Cost.BaseAmount)
                    .ThenBy(typePriority)
                    .First()
                : ctx.DisabledCard;
        return card;
    }

    public static CardTypeData MostExpensive(this IEnumerable<CardTypeData> cards) => cards
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.Cost.BaseAmount)
        .First();
}
