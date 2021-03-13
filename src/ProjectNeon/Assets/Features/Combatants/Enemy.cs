using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int preferredTurnOrder = 99;
    [SerializeField] private int powerLevel = 1;
    [SerializeField] private GameObject prefab;
    [SerializeField] private StringReference deathEffect;
    [SerializeField] private BattleRole battleRole;
    [SerializeField] private EnemyTier tier; 
    [SerializeField] private bool unique;
    
    [SerializeField] private int maxHp;
    [SerializeField] private int maxShield;
    [SerializeField] private int startingShield;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private int leadership;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private float nonStatCardValueFactor;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int startingResourceAmount = 0;
    [SerializeField] private int resourceGainPerTurn = 1;
    [SerializeField] private int cardsPerTurn = 1;
    [SerializeField] private EffectData[] startOfBattleEffects = new EffectData[0];
    [SerializeField] private string lastBalanceDate = "Never";

    public string Name => enemyName;
    public Deck Deck => deck;
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public int PreferredTurnOrder => preferredTurnOrder;
    public GameObject Prefab => prefab;
    public string DeathEffect => deathEffect;
    public BattleRole Role => battleRole;
    public EnemyTier Tier => tier;
    public bool IsUnique => unique;
    public EffectData[] Effects => startOfBattleEffects;
    public bool CanPlayHastyCards => deck.Cards.Any(x => x.TimingType == CardTimingType.Hasty); 
    
    // int stats accessors
    public int MaxHp => maxHp;
    public int MaxShield => maxShield > 0 ? maxShield : toughness * 2;
    public int Toughness => toughness;
    public int Attack => attack;
    public int Magic => magic;
    public int Leadership => leadership;
    public int StartingResourceAmount => startingResourceAmount;
    public int ResourceGainPerTurn => resourceGainPerTurn;
    public int CardsPerTurn => cardsPerTurn;
    
    public Member AsMember(int id)
    {
        var m = new Member(id, enemyName, "Enemy", TeamType.Enemies, Stats, battleRole);
        return m;
    }

    public Member SetupMemberState(Member m, BattleState state)
    {
        var ctx = new EffectContext(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.Members, state.PlayerCardZones);
        m.State.InitResourceAmount(resourceType, startingResourceAmount);
        m.State.ApplyPersistentState(
            new EndOfTurnResourceGainPersistentState(new ResourceQuantity { ResourceType = resourceType.Name, Amount = resourceGainPerTurn}, m));
        startOfBattleEffects?.ForEach(effect => AllEffects.Apply(effect, ctx));
        return m;
    }
    
    public IStats Stats => new StatAddends
        {
            ResourceTypes = resourceType != null ? new IResourceType[] {resourceType} : Array.Empty<IResourceType>()
        }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.MaxShield, maxShield)
        .With(StatType.StartingShield, startingShield)
        .With(StatType.Toughness, toughness)
        .With(StatType.Attack, attack)
        .With(StatType.Magic, magic)
        .With(StatType.Leadership, leadership)
        .With(StatType.Armor, armor)
        .With(StatType.Resistance, resistance)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.ExtraCardPlays, cardsPerTurn);

    public bool IsReadyForPlay => Deck != null && Prefab != null && AI != null;
    
    public Enemy Initialized(string testName, Deck testDeck, TurnAI testAI,  BattleRole testBattleRole, bool testUnique)
    {
        this.enemyName = testName;
        this.deck = testDeck;
        this.ai = testAI;
        this.battleRole = testBattleRole;
        this.unique = testUnique;
        return this;
    }

    public Enemy InitializedStats(Dictionary<string, int> intStats, float testArmor, float testResistance, ResourceType testResourceType)
    {
        this.preferredTurnOrder = intStats["preferredTurnOrder"];
        this.powerLevel = intStats["powerLevel"];
        this.maxHp = intStats["maxHp"];
        this.maxShield = intStats["maxShield"];
        this.toughness = intStats["toughness"];
        this.attack = intStats["attack"];
        this.magic = intStats["magic"];
        this.leadership = intStats["leadership"];
        this.startingResourceAmount = intStats["startingResourceAmount"];
        this.resourceGainPerTurn = intStats["resourceGainPerTurn"];
        this.cardsPerTurn = intStats["cardsPerTurn"];
        this.armor = testArmor;
        this.resistance = testResistance;
        this.resourceType = testResourceType;
        return this;
    }

    public int GetRewardCredits(float powerLevelFactor)
    {
        var typeFactor = battleRole == BattleRole.Boss ? 4 : 1;
        return Mathf.RoundToInt(powerLevel * powerLevelFactor * typeFactor);
    }

    public int GetRewardXp(float powerLevelFactor)
    {
        var typeFactor = battleRole == BattleRole.Boss ? 4 : 1;
        return Mathf.RoundToInt(powerLevel * powerLevelFactor * typeFactor);
    }

    public int CalculatedPowerLevel => 0;
}
