using System.Collections.Generic;
using UnityEngine;

public interface HeroCharacter
{
    string Name { get; }
    Sprite Bust { get; }
    GameObject Body { get; }
    CharacterClass Class { get; }
    Deck Deck { get; }
    CardType ClassCard { get; }
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
    public string Name { get; set; } = "Unknown";
    public Sprite Bust { get; set; }
    public GameObject Body { get; set; }
    public CharacterClass Class { get; set; }
    public Deck Deck { get; set; }
    public CardType ClassCard { get; set; }
    public IStats Stats { get; set; } = new StatAddends();
    public int StartingCredits { get; set; } = 100;
    public HeroFlavorDetails Flavor { get; set; } 
        = new HeroFlavorDetails { HeroDescription = "Desc", RoleDescription = "Desc", BackStory = "BackStory" };
    public HeroSkill[] Skills { get; } = new HeroSkill[0];
    public HeroLevelUpPathway LevelUpTree { get; set; }
    public HashSet<string> Archetypes { get; } = new HashSet<string>();
    public Color Tint { get; } = Color.white;
}

public static class HeroCharacterExtensions
{
    public static bool DeckIsValid(this HeroCharacter h) => h.Deck.Cards.None(x => x == null);
}
