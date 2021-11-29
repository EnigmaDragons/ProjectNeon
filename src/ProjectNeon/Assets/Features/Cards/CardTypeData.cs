using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface CardTypeData 
{
    int Id { get; }
    string Name { get; }
    IResourceAmount Cost { get; }
    CardSpeed Speed { get; }
    Sprite Art  { get; }
    string Description  { get; }
    HashSet<CardTag> Tags  { get; }
    string TypeDescription  { get; }
    CardActionSequence[] ActionSequences  { get; }
    Maybe<CardTypeData> ChainedCard { get; }
    Maybe<CardTypeData> SwappedCard { get; }
    Rarity Rarity { get; }
    HashSet<string> Archetypes { get; }
    Maybe<CardCondition> HighlightCondition { get; }
    Maybe<CardCondition> UnhighlightCondition { get; }
    bool IsSinglePlay { get; }
}

public static class CardTypeDataExtensions
{
    public static bool Is(this CardTypeData c, params CardTag[] tags) => tags.All(tag => c.Tags.Contains(tag));

    public static CardActionsData[] Actions(this CardTypeData c) => c.ActionSequences.Select(a => a.CardActions).ToArray();
    
    public static string ArchetypeDescription(this CardTypeData c) => c.Archetypes.None() 
        ? "General" 
        : string.Join(" - ", c.Archetypes.OrderBy(a => a).Select(a => a.WithSpaceBetweenWords()));
    
    public static string GetArchetypeKey(this CardTypeData c) => c.Archetypes.Any() 
            ? string.Join(" + ", c.Archetypes.OrderBy(a => a)) 
            : "General";
    
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
    {
        return new Card(-1, hero.AsMember(-1), c, hero.Character.Tint, hero.Character.Bust);
    }
    
    public static Card ToNonBattleCard(this CardTypeData c, HeroCharacter hero)
    {
        return new Card(-1, hero.AsMemberForLibrary(), c, hero.Tint, hero.Bust);
    }
}
