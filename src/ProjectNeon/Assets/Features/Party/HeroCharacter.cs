using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface HeroCharacter
{
    int Id { get; }
    string Name { get; }
    
    Sprite Bust { get; }
    GameObject Body { get; }
    string Class { get; }
    BattleRole BattleRole { get; }
    Deck Deck { get; }
    CardTypeData ClassCard { get; }
    IStats Stats { get; }
    int StartingCredits { get; }
    HeroFlavorDetails Flavor { get; }
    HeroSkill[] Skills { get; }
    HeroLevelUpPathway LevelUpTree { get; }
    HashSet<string> Archetypes { get; }
    public Color Tint { get; }
}

public class InMemoryHeroCharacter : HeroCharacter
{
    public int Id { get; set; } = Rng.Int();
    public string Name { get; set; } = "Unknown";
    public Sprite Bust { get; set; }
    public GameObject Body { get; set; }
    public string Class { get; set; }
    public BattleRole BattleRole { get; set; }
    public Deck Deck { get; set; }
    public CardTypeData ClassCard { get; set; }
    public IStats Stats { get; set; } = new StatAddends();
    public int StartingCredits { get; set; } = 100;
    public HeroFlavorDetails Flavor { get; set; } 
        = new HeroFlavorDetails { HeroDescription = "Desc", RoleDescription = "Desc", BackStory = "BackStory" };
    public HeroSkill[] Skills { get; } = new HeroSkill[0];
    public HeroLevelUpPathway LevelUpTree { get; set; }
    public HashSet<string> Archetypes { get; set; } = new HashSet<string>();
    public Color Tint { get; } = Color.white;
}

public static class HeroCharacterExtensions
{
    public static bool DeckIsValid(this HeroCharacter h) => h.Deck.Cards.None(x => x == null);
    
    public static HashSet<string> ArchetypeKeys(this HeroCharacter h)
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
}
