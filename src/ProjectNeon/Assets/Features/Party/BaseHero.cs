using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/1 - HeroCharacter", order = -5)]
public class BaseHero : ScriptableObject, ILocalizeTerms
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private bool allowedForSocialMedia = false;
    [SerializeField] private bool isDisabled = false;
    [SerializeField] private Sprite bust;
    [SerializeField] private Sprite bodySprite;
    [SerializeField] private GameObject body;
    [SerializeField] public string className;
    [SerializeField] private CharacterSex sex;
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
    [SerializeField] public int maxShield = 12;
    [SerializeField] public int startingShield = 0;
    [SerializeField] public int attack = 8;
    [SerializeField] public int magic = 0;
    [SerializeField] public float armor = 0;
    [SerializeField] public float resistance = 0;
    [SerializeField] public int leadership = 0;
    [SerializeField] public int economy = 0;
    [SerializeField] public int startingAegis;
    [SerializeField] public int startingDodge;
    [SerializeField] public int startingTaunt;
    [SerializeField] public ResourceType resource1;
    [SerializeField] public int resource1GainPerTurn;
    [SerializeField] public int resource1StartingAmountOverride = -1;
    [SerializeField] public ResourceType resource2;
    [SerializeField] private CardType[] additionalStartingCards;
    [SerializeField] private CardType[] excludedStartingCards;
    [SerializeField] private EffectData[] startOfBattleEffects = new EffectData[0];

    [SerializeField] public HeroFlavorDetails flavorDetails;
    [SerializeField] private HeroLevelUpPathway levelUpTree;
    [SerializeField] private HeroLevelUpTreeV4 levelUpTreeV4;

    public int Id => id;
    public int ComplexityRating => complexityRating;
    public Sprite Bust => bust;
    public Sprite BodySprite => bodySprite;
    public GameObject Body => body;
    public CharacterSex Sex => sex;
    public MemberMaterialType MaterialType => materialType;
    public BattleRole BattleRole => battleRole;
    public Deck Deck => startingDeck;
    public CardType[] AdditionalStartingCards => additionalStartingCards != null ? additionalStartingCards.ToArray() : Array.Empty<CardType>();    
    public HashSet<CardType> ExcludedCards => excludedStartingCards != null 
        ? new HashSet<CardType>(excludedStartingCards.Concat(BasicCard)) 
        : new HashSet<CardType>(BasicCard.AsArray());
    public CardType BasicCard => basic;
    public int StartingCredits => startingCredits;
    public HeroLevelUpPathway LevelUpTree => levelUpTree;
    public HeroLevelUpTreeV4 LevelUpTreeV4 => levelUpTreeV4;
    public HashSet<string> Archetypes => new HashSet<string>(archetypes.Select(x => x.Value));
    public Color Tint => tint;
    public CharacterAnimations Animations => animations;
    public CharacterAnimationSoundSet AnimationSounds => animationSounds;
    public bool IsAllowedForSocialMedia => allowedForSocialMedia;
    public bool IsDisabled => isDisabled;
    public EffectData[] StartOfBattleEffects => startOfBattleEffects;
    
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

    public CardType[] ParagonCards => 
        levelUpTreeV4 != null
            ? levelUpTreeV4.ParagonCards
            : levelUpTree != null 
                ? levelUpTree
                    .ForLevel(5)
                    .OfType<NewBasicLevelUpOption>()
                    .Select(o => o.Card)
                    .ToArray()
                : Array.Empty<CardType>();
    
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

    private InMemoryResourceType[] GetResourceTypes()
    {
        var resourceCount = 0;
        if (resource1 != null)
            resourceCount++;
        if (resource2 != null)
            resourceCount++;
        var rt = new InMemoryResourceType[resourceCount];
        if (resource1 != null)
        {
            rt[0] = resource1StartingAmountOverride > -1
                ? resource1.WithAmounts(resource1StartingAmountOverride)
                : new InMemoryResourceType(resource1);
        }
        if (resource2 != null)
            rt[1] = new InMemoryResourceType(resource2);
        return rt;
    }

    public int AdventuresPlayedBeforeUnlocked => adventuresPlayedBeforeUnlocked;

    public string[] GetLocalizeTerms()
        => new[] {this.NameTerm(), this.ClassTerm(), this.DescriptionTerm(), this.BackStoryTerm()}
            .Concat(resource1 == null ? new string[0] : new [] { resource1.GetTerm() })
            .Concat(resource2 == null ? new string[0] : new [] { resource2.GetTerm() })
            .ToArray();
}
