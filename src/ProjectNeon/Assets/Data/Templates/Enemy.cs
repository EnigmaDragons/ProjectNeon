using System;
using UnityEngine;

public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int powerLevel;
    [SerializeField] private GameObject prefab;
    
    [SerializeField] private int maxHp;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType resourceType;

    public Deck Deck => deck;
    public Member AsMember(int id) => new Member(id, enemyName, "Enemy", TeamType.Enemies, Stats);
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public GameObject Prefab => prefab;
    
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
        .With(StatType.Damagability, 1f);
}
