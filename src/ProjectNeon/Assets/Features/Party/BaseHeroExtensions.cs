using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BaseHeroExtensions
{
    public static bool DeckIsValid(this BaseHero h) => h.Deck.Cards.None(x => x == null);
    
    public static HashSet<string> ArchetypeKeys(this BaseHero h)
    {
        var archetypes = h.Archetypes.OrderBy(a => a).ToList();
        var archetypeKeys = new HashSet<string>();
        // Singles
        archetypes.ForEach(a => archetypeKeys.Add(a));
        // Duals
        archetypes.Combinations(2)
            .Select(p => string.Join(" + ", p))
            .ForEach(a => archetypeKeys.Add(a));
        // Triple
        archetypeKeys.Add(string.Join(" + ", archetypes));
        return archetypeKeys;
    }

    public static Member AsMemberForLibrary(this BaseHero h) => AsMemberForLibrary(h, h.Stats);
    public static Member AsMemberForLibrary(this BaseHero h, IStats stats)
    {
        var m = new Member(-1, h.NameTerm(), h.ClassTerm(), h.MaterialType, TeamType.Party, stats, h.BattleRole, stats.DefaultPrimaryStat(stats), true, false, stats.MaxHp(), h.BasicCard);
        h.CounterAdjustments.ForEach(c => m.State.Adjust(c.Key, c.Value));
        return m;
    }

    public static CardType[] StartingCards(this BaseHero hero, ShopCardPool allCards) => allCards
        .Get(hero.Archetypes, new HashSet<int>(), Rarity.Starter)
        .Concat(hero.AdditionalStartingCards)
        .Except(hero.ExcludedCards)
        .NumCopies(4)
        .ToArray();

    public static string NameTerm(this BaseHero hero) => $"Heroes/HeroName{hero.Id}";
    public static string ClassTerm(this BaseHero hero) => $"Heroes/HeroClass{hero.Id}";
    public static string DescriptionTerm(this BaseHero hero) => $"Heroes/HeroDescription{hero.Id}";
    public static string BackStoryTerm(this BaseHero hero) => $"Heroes/HeroBackStory{hero.Id}";
}
