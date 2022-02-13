using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface HeroCharacter
{
    int Id { get; }
    string Name { get; }
    int ComplexityRating { get; }
    
    Sprite Bust { get; }
    GameObject Body { get; }
    MemberMaterialType MaterialType { get; }
    string Class { get; }
    BattleRole BattleRole { get; }
    Deck Deck { get; }
    CardTypeData BasicCard { get; }
    CardTypeData[] ParagonCards { get; }
    HashSet<CardTypeData> ExcludedCards { get; }
    IStats Stats { get; }
    int StartingCredits { get; }
    HeroFlavorDetails Flavor { get; }
    HeroLevelUpPathway LevelUpTree { get; }
    HeroLevelUpTreeV4 LevelUpTreeV4 { get; }
    HashSet<string> Archetypes { get; }
    Color Tint { get; }
    CharacterAnimations Animations { get; }
    Dictionary<string, int> CounterAdjustments { get; }

    void SetupMemberState(Member m, BattleState s);
}

public class InMemoryHeroCharacter : HeroCharacter
{
    public int Id { get; set; } = Rng.Int();
    public string Name { get; set; } = "Unknown";
    public int ComplexityRating { get; set; } = 0;
    public Sprite Bust { get; set; }
    public GameObject Body { get; set; }
    public MemberMaterialType MaterialType { get; } = MemberMaterialType.Unknown;
    public string Class { get; set; }
    public BattleRole BattleRole { get; set; }
    public Deck Deck { get; set; }
    public CardTypeData BasicCard { get; set; }
    public CardTypeData[] ParagonCards { get; set; } = new CardTypeData[0];
    public HashSet<CardTypeData> ExcludedCards { get; set; } = new HashSet<CardTypeData>();
    public IStats Stats { get; set; } = new StatAddends();
    public int StartingCredits { get; set; } = 100;
    public HeroFlavorDetails Flavor { get; set; } 
        = new HeroFlavorDetails { HeroDescription = "Desc", RoleDescription = "Desc", BackStory = "BackStory" };
    public HeroLevelUpPathway LevelUpTree { get; set; }
    public HeroLevelUpTreeV4 LevelUpTreeV4 { get; set; }
    public HashSet<string> Archetypes { get; set; } = new HashSet<string>();
    public Color Tint { get; } = Color.white;
    public CharacterAnimations Animations { get; }
    public Dictionary<string, int> CounterAdjustments { get; } = new Dictionary<string, int>();
    public void SetupMemberState(Member m, BattleState s) {}
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
    
    public static Member AsMemberForLibrary(this HeroCharacter h)
    {
        var stats = h.Stats;
        var m = new Member(-1, h.Name, h.Class, h.MaterialType, TeamType.Party, stats, h.BattleRole, stats.DefaultPrimaryStat(stats), stats.MaxHp(), Maybe<CardTypeData>.Present(h.BasicCard));
        h.CounterAdjustments.ForEach(c => m.State.Adjust(c.Key, c.Value));
        return m;
    }

    public static string DisplayName(this HeroCharacter character) => Localize.GetHero(character.Name);
}
