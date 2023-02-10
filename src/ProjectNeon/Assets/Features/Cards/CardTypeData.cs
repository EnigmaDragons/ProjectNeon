using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface CardTypeData 
{
    int Id { get; }
    string Name { get; }
    string NameTerm { get; }
    IResourceAmount Cost { get; }
    CardSpeed Speed { get; }
    Sprite Art  { get; }
    string Description  { get; }
    string DescriptionTerm { get; }
    CardDescriptionV2 DescriptionV2 { get; }
    HashSet<CardTag> Tags  { get; }
    string TypeDescription  { get; }
    CardActionSequence[] ActionSequences  { get; }
    Maybe<CardTypeData> ChainedCard { get; }
    Maybe<CardTypeData> SwappedCard { get; }
    Rarity Rarity { get; }
    HashSet<string> Archetypes { get; }
    Maybe<CardCondition> HighlightCondition { get; }
    Maybe<CardCondition> UnhighlightCondition { get; }
    Maybe<TargetedCardCondition> TargetedHighlightCondition { get; }
    Maybe<TargetedCardCondition> TargetedUnhighlightCondition { get; }
    bool IsSinglePlay { get; }
}

public static class CardTypeDataExtensions
{
    public static bool Is(this CardTypeData c, params CardTag[] tags) => tags.All(tag => c.Tags.Contains(tag));
    public static bool IsAoE(this CardTypeData c) => c.ActionSequences.AnyNonAlloc() && c.ActionSequences[0].Scope == Scope.All;

    public static CardActionsData[] Actions(this CardTypeData c) => c.ActionSequences.Select(a => a.CardActions).ToArray();
    
    public static string LocalizedArchetypeDescription(this CardTypeData c) 
        => string.Join(" - ", c.Archetypes().Select(Localized.Archetype));
    
    public static string ArchetypeDescription(this CardTypeData c) => c.Archetypes.None() 
        ? "General" 
        : string.Join(" - ", c.Archetypes.OrderBy(a => a).Select(a => a.WithSpaceBetweenWords()));

    public static string GetArchetypeKey(this CardTypeData c) => string.Join(" + ", c.Archetypes());
    
    private static List<string> Archetypes(this CardTypeData c) =>
        c.Archetypes.AnyNonAlloc() 
            ? c.Archetypes.OrderBy(a => a).ToList() 
            : new List<string>{"General"};
    
    public static Card CreateInstance(this CardTypeData c, int id, Member owner) => new Card(id, owner, c, Maybe<Color>.Missing(), Maybe<Sprite>.Missing());
    public static Card CreateInstance(this CardTypeData c, int id, Member owner, Maybe<Color> tint, Maybe<Sprite> bust) => new Card(id, owner, c, tint, bust);

    public static IEnumerable<EffectData> BattleEffects(this CardTypeData c) => c.ActionSequences.SelectMany(
        a => a.CardActions.BattleEffects.Concat(a.CardActions.ConditionalBattleEffects));
    
    public static IEnumerable<ActionConditionData> Conditions(this CardTypeData c) => c.ActionSequences
        .SelectMany(a => a.CardActions.Actions.Where(x => x.Type == CardBattleActionType.Condition)
            .Select(x => x.ConditionData));

    public static IEnumerable<EffectData> ReactionBattleEffects(this CardTypeData card) 
        => card.Actions()
                .SelectMany(a => a.BattleEffects)
                .Where(b => b.IsReactionCard)
                .SelectMany(c => c.ReactionSequence.ActionSequence.CardActions.BattleEffects)
        .Concat(
            card.Actions()
                .SelectMany(a => a.BattleEffects)
                .Where(b => b.IsReactionEffect)
                .Where(c => c.ReactionEffect.CardActions != null)
                .SelectMany(c => c.ReactionEffect.CardActions.BattleEffects));

    public static void ShowDetailedCardView(this CardTypeData c) => Message.Publish(new ShowDetailedCardView(c));

    public static Card ToNonBattleCard(this CardTypeData c, PartyAdventureState party)
    {
        var hero = party.BestMatchFor(c.GetArchetypeKey());
        return ToNonBattleCard(c, hero);
    }

    public static Card ToNonBattleCard(this CardTypeData c, Hero hero)
        => ToNonBattleCard(c, hero.Character, hero.Stats);

    public static Card ToNonBattleCard(this CardTypeData c, HeroCharacter hero, IStats heroStats) 
        => new Card(-1, hero.AsMemberForLibrary(heroStats), c, c.NonBattleTint(hero), c.NonBattleBust(hero));

    private static Color NonBattleTint(this CardTypeData c, HeroCharacter h)
        => c.Archetypes.AnyNonAlloc() ? h.Tint : Color.white;
    
    private static Maybe<Sprite> NonBattleBust(this CardTypeData c, HeroCharacter h)
        => c.Archetypes.AnyNonAlloc() ? new Maybe<Sprite>(h.Bust) : Maybe<Sprite>.Missing();

    public static CardType[] ReferencedCards(this CardTypeData c)
    {
        var list = new List<CardType>();
        c.ChainedCard.IfPresentAndMatches(x => x is CardType, x => list.Add((CardType)x));
        c.SwappedCard.IfPresentAndMatches(x => x is CardType, x => list.Add((CardType)x));
        return list.ToArray();
    }
}
