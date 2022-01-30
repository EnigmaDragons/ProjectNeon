using System;
using System.Collections.Generic;
using System.Linq;

public static class AICardSelectionLogic
{
    public static CardSelectionContext WithSelectedCardByNameIfPresent(this CardSelectionContext ctx, string cardName)
        => (ctx.SelectedCard.IsMissing 
           && ctx.CardOptions.Any(c => c.Name.Equals(cardName))
            ? ctx.WithSelectedCard(ctx.CardOptions.First(c => c.Name.Equals(cardName)))
            : ctx).WithLoggedSelection(nameof(WithSelectedCardByNameIfPresent));

    public static CardSelectionContext WithSelectedFocusCardIfApplicable(this CardSelectionContext ctx)
        => (ctx.SelectedCard.IsMissing 
           && ctx.CardOptions.None(c => c.Is(CardTag.Ultimate) && c.IsAoE()) 
           && ctx.CardOptions.Any(c => c.Is(CardTag.Focus)) && ctx.FocusTarget.IsMissing
            ? ctx.WithSelectedCard(SelectFocusCard(ctx))
            : ctx).WithLoggedSelection(nameof(WithSelectedFocusCardIfApplicable));

    public static CardSelectionContext WithSelectedDesignatedAttackerCardIfApplicable(this CardSelectionContext ctx) 
        => (ctx.SelectedCard.IsMissing 
           && ctx.Strategy.DesignatedAttacker.Equals(ctx.Member) 
           && ctx.CardOptions.Any(p => p.Is(CardTag.Attack))
            ? ctx.WithSelectedCard(ctx.SelectAttackCard())
            : ctx).WithLoggedSelection(nameof(WithSelectedDesignatedAttackerCardIfApplicable));

    public static CardSelectionContext WithSelectedUltimateIfAvailable(this CardSelectionContext ctx)
        => (ctx.SelectedCard.IsMissing 
           && ctx.CardOptions.Any(c => c.Tags.Contains(CardTag.Ultimate))
            ? ctx.WithSelectedCard(ctx.CardOptions.Where(c => c.Tags.Contains(CardTag.Ultimate)).MostExpensive())
            : ctx).WithLoggedSelection(nameof(WithSelectedUltimateIfAvailable));

    public static CardSelectionContext WithCommonSenseSelections(this CardSelectionContext ctx)
        => ctx
            .PlayAntiStealthCardIfAllEnemiesAreStealthed()
            .DontPlayRequiresFocusCardWithoutAFocusTarget()
            .DontPlayFocusCardIfFocusTargetAlreadySelected()
            .DontPlayAntiStealthCardIfNoEnemiesAreStealthed()
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
            .DontPlayResistanceIfEnemiesDontHaveMagic()
            .DontPlayDamageOverTimeDefenseIfAlliesAreTooLow()
            .DontPlayArmorIfEnemiesDontHaveAttack()
            .DontGiveAlliesDodgeIfTheyAlreadyHaveEnough()
            .DontGiveAlliesAegisIfTheyAlreadyHaveEnough()
            .DontStealCreditsIfOpponentDoesntHaveAny();

    public static CardSelectionContext DontPlayRequiresFocusCardWithoutAFocusTarget(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(_ => ctx.FocusTarget.IsMissing, CardTag.RequiresFocus);
    
    public static CardSelectionContext DontPlayFocusCardIfFocusTargetAlreadySelected(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(_ => ctx.FocusTarget.IsPresent, CardTag.Focus);    
    
    public static CardSelectionContext PlayAntiStealthCardIfAllEnemiesAreStealthed(this CardSelectionContext ctx)
        => ctx.IfTruePlayType(x => x.Enemies.All(e => e.IsStealthed()), CardTag.AntiStealth);
    
    public static CardSelectionContext DontPlayAntiStealthCardIfNoEnemiesAreStealthed(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.None(e => e.IsStealthed()), CardTag.AntiStealth);
    
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
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.CurrentShield() > a.MaxShield() * 0.6), CardTag.Defense, CardTag.Shield);

    private static CardSelectionContext DontPlayDamageOverTimeDefenseIfAlliesAreTooLow(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Allies.All(a => a.CurrentHp() < 6), CardTag.Defense, CardTag.DamageOverTime);

    private static CardSelectionContext DontPlayResistanceIfEnemiesDontHaveMagic(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.State[StatType.Magic] < 5), CardTag.Defense, CardTag.Resistance);
    
    private static CardSelectionContext DontPlayArmorIfEnemiesDontHaveAttack(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.State[StatType.Attack] < 5), CardTag.Defense, CardTag.Armor);

    private static CardSelectionContext DontRemoveResourcesIfOpponentsDontHaveMany(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.Enemies.All(e => e.State.PrimaryResourceValue < 1f), CardTag.RemoveResources);

    private static CardSelectionContext DontStealCreditsIfOpponentDoesntHaveAny(this CardSelectionContext ctx)
        => ctx.IfTrueDontPlayType(x => x.PartyAdventureState.Credits <= 0, CardTag.StealCredits);

    private static CardTypeData SelectAttackCard(this CardSelectionContext ctx) 
        => ctx.CardOptions.Where(o => o.Is(CardTag.Attack))
            .Select((o, i) => (option: o, index: i))
            .OrderByDescending(c => SmartCardPreference(ctx, c.option, c.index))
            .First().option;

    private static CardTypeData SelectFocusCard(this CardSelectionContext ctx)
        => ctx.CardOptions.Where(o => o.Is(CardTag.Focus)).ToArray().Shuffled().First();

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx)
        => ctx.WithFinalizedCardSelection((_, __) => 0);

    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, params CardTag[] tagPriority)
    {
        var dictionary = new DictionaryWithDefault<CardTag, int>(99);
        for (int i = 1; i < tagPriority.Length + 1; i++)
            dictionary[tagPriority[i - 1]] = i;
        return ctx.WithFinalizedCardSelection((c, i) => dictionary[c.Tags.First()]);
    }
    
    public static CardSelectionContext WithFinalizedCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int, int> typePriority)
        => (ctx.SelectedCard.IsMissing
            ? ctx.WithSelectedCard(FinalizeCardSelection(ctx, typePriority))
            : ctx).WithLoggedSelection(nameof(WithFinalizedCardSelection));
    
    private static Maybe<CardTypeData> _lastCard = Maybe<CardTypeData>.Missing();
    private static CardSelectionContext WithLoggedSelection(this CardSelectionContext ctx, string selectionPhase)
    {
        if (_lastCard.IsMissing && ctx.SelectedCard.IsPresent)
            DevLog.Write($"Selected Card in {selectionPhase}: {ctx.SelectedCard.Value.Name}");

        _lastCard = ctx.SelectedCard;
        return ctx;
    }

    private static readonly int SuperHighlyUnpreferred = -999;
    private static readonly int HighlyUnpreferred = -100;
    private static readonly int MediumUnpreferred = -50;
    private static readonly int SlightlyUnpreferred = -30;
    private static readonly int DefaultPreference = -20;
    private static readonly int SlightlyPreferred = 30;
    private static readonly int MediumPreferred = 50;
    private static readonly int HighlyPreferred = 100;
    
    private static int SmartCardPreference(CardSelectionContext ctx, CardTypeData card, int optionIndex)
    {
        var cardAction = card.ActionSequences.First();
        var preferenceScore = DefaultPreference;
        
        if (ctx.UnhighlightedCardOptions.Contains(card))
            preferenceScore += SuperHighlyUnpreferred;
        
        if (card.Is(CardTag.Unpreferred))
            preferenceScore += HighlyUnpreferred;
        if (ctx.AiPreferences.UnpreferredCardTags.Any(t => card.Is(t)))
            preferenceScore += HighlyUnpreferred;
        
        if (card.Is(CardTag.Ultimate))
            preferenceScore += MediumPreferred;
        
        if (ctx.Enemies.Length == 1 && cardAction.Scope == Scope.All && cardAction.Group == Group.Opponent)
            preferenceScore += MediumUnpreferred;
        if (card.Is(CardTag.DoubleDamage) && cardAction.Group == Group.Self && ctx.Member.HasDoubleDamage())
            preferenceScore += MediumUnpreferred;
        
        if (card.Is(CardTag.BuffAttack) && cardAction.Group == Group.Self && ctx.Member.HasAttackBuff())
            preferenceScore += SlightlyUnpreferred;
        if (ctx.LastPlayedCard.IsPresentAnd(lastCard => lastCard.Id == card.Id))
            preferenceScore += SlightlyUnpreferred;

        var cardTagPriorities = ctx.AiPreferences.CardTagPriority.Reverse().ToArray();
        for(var i = 0; i < cardTagPriorities.Length; i++)
            if (card.Is(cardTagPriorities[i]))
                preferenceScore += (i + 1) * 5;
        
        if (ctx.LastPlayedCard.IsPresent && ctx.AiPreferences.RotatePlayingCardTags.Any())
            preferenceScore += RotatingCardTagPreferenceAmount(ctx, card);

        preferenceScore += (ctx.CardOptions.Count() - (optionIndex + 1)) * ctx.AiPreferences.CardOrderPreferenceFactor;
        
        return preferenceScore;
    }

    private static int RotatingCardTagPreferenceAmount(this CardSelectionContext ctx, CardTypeData card)
    {
        var lastCard = ctx.LastPlayedCard.Value;
        var rotateTags = ctx.AiPreferences.RotatePlayingCardTags;
        var unpreferredTag = Maybe<CardTag>.Missing();
        var preferredTag = Maybe<CardTag>.Missing();
            
        for (var i = 0; i < rotateTags.Length; i++)
        {
            var currentTag = rotateTags[i];
            if (!lastCard.Tags.Contains(currentTag)) continue;
                
            unpreferredTag = currentTag;
            preferredTag = rotateTags[(i + 1) % rotateTags.Length];
        }
            
        if (unpreferredTag.IsPresentAnd(t => card.Tags.Contains(t)))
            return HighlyUnpreferred;
        if (preferredTag.IsPresentAnd(t => card.Tags.Contains(t)))
            return MediumPreferred;
        return 0;
    }
    
    public static CardSelectionContext WithFinalizedSmartCardSelection(this CardSelectionContext ctx)
        => ctx.WithFinalizedCardSelection((c, index) => SmartCardPreference(ctx, c, index));

    private static CardTypeData FinalizeCardSelection(this CardSelectionContext ctx, Func<CardTypeData, int, int> typePriority)
    {
        if (ctx.SelectedCard.IsPresent)
            return ctx.SelectedCard.Value;
        if (ctx.CardOptions.None())
            return ctx.SpecialCards.DisabledCard;

        var cardPreferenceOrder = ctx.CardOptions
            .Select((card, index) => (card, typePriority: typePriority(card, index)))
            .ToArray()
            .Shuffled()
            .OrderByDescending(o => o.typePriority)
            .ThenBy(o => o.card.Cost.BaseAmount)
            .ToList();
        
        DevLog.Write($"Card Preference Order: {string.Join(", ", cardPreferenceOrder.Select(c => $"{c.card.Name} {c.typePriority}"))}");
        
        return cardPreferenceOrder.First().card;
    }
    
    public static CardSelectionContext WithPhases(this CardSelectionContext ctx)
    {
        var phase = ctx.Member.State[TemporalStatType.Phase].CeilingInt();
        var phaselessCards = ctx.CardOptions.Where(x => !x.Tags.Contains(CardTag.Phase1) && !x.Tags.Contains(CardTag.Phase2) && !x.Tags.Contains(CardTag.Phase3));
        if (phase == 1)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase1))));
        if (phase == 2)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase2))));
        if (phase == 3)
            return ctx.WithCardOptions(phaselessCards.Concat(ctx.CardOptions.Where(x => x.Tags.Contains(CardTag.Phase3))));
        return ctx;
    }

    public static CardTypeData MostExpensive(this IEnumerable<CardTypeData> cards) => cards
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.Cost.BaseAmount)
        .First();
}
