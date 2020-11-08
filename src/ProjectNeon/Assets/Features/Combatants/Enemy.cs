using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int preferredTurnOrder = 99;
    [SerializeField] private int powerLevel = 1;
    [SerializeField] private int rewardCredits = 25;
    [SerializeField] private GameObject prefab;
    [SerializeField] private StringReference deathEffect;
    [SerializeField] private BattleRole battleRole;
    [SerializeField] private bool unique;
    
    [SerializeField] private int maxHp;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int startingResourceAmount = 0;
    [SerializeField] private int resourceGainPerTurn = 1;
    [SerializeField] private int cardsPerTurn = 1;
    [SerializeField] private EffectData[] startOfBattleEffects;

    public string Name => enemyName;
    public Deck Deck => deck;
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public int PreferredTurnOrder => preferredTurnOrder;
    public int RewardCredits => rewardCredits;
    public GameObject Prefab => prefab;
    public string DeathEffect => deathEffect;
    public BattleRole Role => battleRole;
    public bool IsUnique => unique;
    
    // int stats accessors
    public int MaxHp => maxHp;
    public int Toughness => toughness;
    public int Attack => attack;
    public int Magic => magic;
    public int StartingResourceAmount => startingResourceAmount;
    public int ResourceGainPerTurn => resourceGainPerTurn;
    public int CardsPerTurn => cardsPerTurn;

    public Member AsMember(int id)
    {
        var m = new Member(id, enemyName, "Enemy", TeamType.Enemies, Stats, battleRole);
        m.State.InitResourceAmount(resourceType, startingResourceAmount);
        m.State.ApplyPersistentState(
            new EndOfTurnResourceGainPersistentState(new ResourceQuantity { ResourceType = resourceType.Name, Amount = resourceGainPerTurn}, m));
        startOfBattleEffects.ForEach(effect => AllEffects.Apply(effect, new EffectContext(m, new Single(m))));
        return m;
    }

    public IStats Stats => new StatAddends
        {
            ResourceTypes = resourceType != null ? new IResourceType[] {resourceType} : Array.Empty<IResourceType>()
        }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.Toughness, toughness)
        .With(StatType.Attack, attack)
        .With(StatType.Magic, magic)
        .With(StatType.Armor, armor)
        .With(StatType.Resistance, resistance)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.ExtraCardPlays, cardsPerTurn);

    public bool IsReadyForPlay => Deck != null && Prefab != null;
    
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
        this.rewardCredits = intStats["rewardCredits"];
        this.maxHp = intStats["maxHp"];
        this.toughness = intStats["toughness"];
        this.attack = intStats["attack"];
        this.magic = intStats["magic"];
        this.startingResourceAmount = intStats["startingResourceAmount"];
        this.resourceGainPerTurn = intStats["resourceGainPerTurn"];
        this.cardsPerTurn = intStats["cardsPerTurn"];
        this.armor = testArmor;
        this.resistance = testResistance;
        this.resourceType = testResourceType;
        return this;
    }
}
