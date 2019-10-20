using System;
using UnityEngine;

public class Hero : ScriptableObject
{
    [SerializeField] private Sprite bust;
    [SerializeField] private StringVariable className;
    
    // Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int toughness;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType resource1;
    
    public Sprite Bust => bust;
    public StringVariable ClassName => className;

    public IStats Stats => new StatAddends
        {
            ResourceTypes = resource1 != null ? new IResourceType[] {resource1} : Array.Empty<IResourceType>()
        }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.Toughness, toughness)
        .With(StatType.Attack, attack)
        .With(StatType.Magic, magic)
        .With(StatType.Armor, armor)
        .With(StatType.Resistance, resistance)
        .With(StatType.Damagability, 1f);
}
