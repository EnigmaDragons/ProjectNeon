using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/HeroCharacter")]
public class BaseHero : ScriptableObject, HeroCharacter
{
    [SerializeField] private Sprite bust;
    [SerializeField] private GameObject body;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private Deck startingDeck;
    [SerializeField] private Color tint;
    [SerializeField] private int startingCredits = 100;
    
    // Stats
    [SerializeField] private int maxHp = 40;
    [SerializeField] private int maxShield = 12;
    [SerializeField] private int startingShield = 0;
    [SerializeField] private int toughness = 6;
    [SerializeField] private int attack = 8;
    [SerializeField] private int magic = 0;
    [SerializeField] private float armor = 0;
    [SerializeField] private float resistance = 0;
    [SerializeField] private int leadership = 0;
    [SerializeField] private ResourceType resource1;
    [SerializeField] private ResourceType resource2;
    [SerializeField] private HeroSkill[] skills;
    [SerializeField] private CardType[] additionalStartingCards;

    [SerializeField] private HeroFlavorDetails flavorDetails;
    [SerializeField] private HeroLevelUpPathway levelUpTree;

    public string Name => name;
    public Sprite Bust => bust;
    public GameObject Body => body;
    public CharacterClass Class => characterClass;
    public Deck Deck => startingDeck;
    public CardType[] AdditionalStartingCards => additionalStartingCards ?? new CardType[0];
    public CardType ClassCard => Class.BasicCard;
    public int StartingCredits => startingCredits;
    public HeroSkill[] Skills => skills;
    public HeroLevelUpPathway LevelUpTree => levelUpTree;
    public HeroFlavorDetails Flavor => flavorDetails;
    public HashSet<string> Archetypes => new HashSet<string>(archetypes.Select(x => x.Value));
    public Color Tint => tint;

    public IStats Stats => new StatAddends
        {
            ResourceTypes = resource1 != null 
                ? resource2 != null 
                    ? new IResourceType[] {resource1, resource2} 
                    : new IResourceType[] {resource1} 
                : Array.Empty<IResourceType>()
        }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.MaxShield, maxShield)
        .With(StatType.StartingShield, startingShield)
        .With(StatType.Toughness, toughness)
        .With(StatType.Attack, attack)
        .With(StatType.Magic, magic)
        .With(StatType.Armor, armor)
        .With(StatType.Resistance, resistance)
        .With(StatType.Leadership, leadership)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f);
}
