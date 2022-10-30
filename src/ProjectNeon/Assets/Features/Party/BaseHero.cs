﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/1 - HeroCharacter", order = -5)]
public class BaseHero : ScriptableObject, HeroCharacter
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private bool allowedForSocialMedia = false;
    [SerializeField] private bool isDisabled = false;
    [SerializeField] private Sprite bust;
    [SerializeField] private GameObject body;
    [SerializeField] private StringReference className;
    [SerializeField] private MemberMaterialType materialType;
    [SerializeField] private BattleRole battleRole;
    [SerializeField] private CardType basic;
    [SerializeField] private StringVariable[] archetypes;
    [SerializeField] private Deck startingDeck;
    [SerializeField] private Color tint;
    [SerializeField] private CharacterAnimations animations;
    [SerializeField] private CharacterAnimationSoundSet animationSounds;
    [SerializeField] private int startingCredits = 100;
    [SerializeField, Range(1, 5)] private int complexityRating = 3;
    [SerializeField] private int adventuresPlayedBeforeUnlocked;

    // Stats
    [SerializeField] public int maxHp = 40;
    [SerializeField] private int maxShield = 12;
    [SerializeField] private int startingShield = 0;
    [SerializeField] private int attack = 8;
    [SerializeField] private int magic = 0;
    [SerializeField] private float armor = 0;
    [SerializeField] private float resistance = 0;
    [SerializeField] private int leadership = 0;
    [SerializeField] private int economy = 0;
    [SerializeField] private int startingAegis;
    [SerializeField] private int startingDodge;
    [SerializeField] private int startingTaunt;
    [SerializeField] private ResourceType resource1;
    [SerializeField] private int resource1GainPerTurn;
    [SerializeField] private int resource1StartingAmountOverride = -1;
    [SerializeField] private ResourceType resource2;
    [SerializeField] private CardType[] additionalStartingCards;
    [SerializeField] private CardType[] excludedStartingCards;

    [SerializeField] private HeroFlavorDetails flavorDetails;
    [SerializeField] private HeroLevelUpPathway levelUpTree;
    [SerializeField] private HeroLevelUpTreeV4 levelUpTreeV4;

    public int Id => id;
    public int ComplexityRating => complexityRating;
    public Sprite Bust => bust;
    public GameObject Body => body;
    public MemberMaterialType MaterialType => materialType;
    public string Class => className;
    public BattleRole BattleRole => battleRole;
    public Deck Deck => startingDeck;
    public CardTypeData[] AdditionalStartingCards => additionalStartingCards != null ? additionalStartingCards.Cast<CardTypeData>().ToArray() : Array.Empty<CardTypeData>();    
    public HashSet<CardTypeData> ExcludedCards => excludedStartingCards != null 
        ? new HashSet<CardTypeData>(excludedStartingCards.Concat(BasicCard)) 
        : new HashSet<CardTypeData>(BasicCard.AsArray());
    public CardTypeData BasicCard => basic;
    public int StartingCredits => startingCredits;
    public HeroLevelUpPathway LevelUpTree => levelUpTree;
    public HeroLevelUpTreeV4 LevelUpTreeV4 => levelUpTreeV4;
    public HeroFlavorDetails Flavor => flavorDetails;
    public HashSet<string> Archetypes => new HashSet<string>(archetypes.Select(x => x.Value));
    public Color Tint => tint;
    public CharacterAnimations Animations => animations;
    public CharacterAnimationSoundSet AnimationSounds => animationSounds;
    public bool IsAllowedForSocialMedia => allowedForSocialMedia;
    public bool IsDisabled => isDisabled;
    
    public IStats Stats => new StatAddends { ResourceTypes = GetResourceTypes() }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.MaxShield, maxShield)
        .With(StatType.StartingShield, startingShield)
        .With(StatType.Attack, attack)
        .With(StatType.Magic, magic)
        .With(StatType.Armor, armor)
        .With(StatType.Resistance, resistance)
        .With(StatType.Leadership, leadership)
        .With(StatType.Economy, economy)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f);
    
    public HashSet<string> ArchetypeKeys
    {
        get
        {
            var archetypes = Archetypes.OrderBy(a => a).ToList();
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

    public CardTypeData[] ParagonCards => 
        levelUpTreeV4 != null
            ? levelUpTreeV4.ParagonCards
            : levelUpTree != null 
                ? levelUpTree
                    .ForLevel(5)
                    .OfType<NewBasicLevelUpOption>()
                    .Select(o => o.Card)
                    .ToArray()
                : new CardTypeData[0];
    
    public Dictionary<string, int> CounterAdjustments => new Dictionary<string, int>
    {
        {TemporalStatType.Aegis.ToString(), startingAegis},
        {TemporalStatType.Dodge.ToString(), startingDodge},
        {TemporalStatType.Taunt.ToString(), startingTaunt},
    };

    public IResourceAmount[] TurnResourceGains => resource1 != null
        ? new IResourceAmount[1] { new InMemoryResourceAmount(resource1GainPerTurn, resource1, false) }
        : new IResourceAmount[0]; 

    public void SetupMemberState(Member m, BattleState s)
    {
        m.State.ApplyPersistentState(new EndOfTurnResourceGainPersistentState(new ResourceQuantity { ResourceType = resource1.Name, Amount = resource1GainPerTurn}, m, s.Party));
    }

    private IResourceType[] GetResourceTypes()
    {
        var resourceCount = 0;
        if (resource1 != null)
            resourceCount++;
        if (resource2 != null)
            resourceCount++;
        var rt = new IResourceType[resourceCount];
        if (resource1 != null)
        {
            rt[0] = resource1StartingAmountOverride > -1
                ? resource1.WithAmounts(resource1StartingAmountOverride)
                : resource1;
        }
        if (resource2 != null)
            rt[1] = resource2;
        return rt;
    }

    public int AdventuresPlayedBeforeUnlocked => adventuresPlayedBeforeUnlocked;
}
