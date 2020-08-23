using System;
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
    
    [SerializeField] private int maxHp;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int startingResourceAmount;
    [SerializeField] private int cardsPerTurn = 1;

    public string Name => enemyName;
    public Deck Deck => deck;
    public Member AsMember(int id)
    {
        var m = new Member(id, enemyName, "Enemy", TeamType.Enemies, Stats, battleRole);
        m.State.InitResourceAmount(resourceType, startingResourceAmount);
        return m;
    }

    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public int PreferredTurnOrder => preferredTurnOrder;
    public int RewardCredits => rewardCredits;
    public GameObject Prefab => prefab;
    public string DeathEffect => deathEffect;

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
        .With(StatType.ExtraCardPlays, cardsPerTurn);

    public bool IsReadyForPlay => Deck != null;
}
