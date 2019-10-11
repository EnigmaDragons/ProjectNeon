using System;
using Features.Combatants;
using UnityEngine;

public class Hero : ScriptableObject
{
    [SerializeField] private Sprite bust;
    [SerializeField] private StringVariable className;
    
    // Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int magic;
    [SerializeField] private float armor;
    [SerializeField] private float resistance;
    [SerializeField] private ResourceType resource1;
    
    public Sprite Bust => bust;
    public StringVariable ClassName => className;
    
    public IStats Stats => new InMemoryStats
    {
        MaxHP = maxHp,
        Magic = magic,
        Attack = attack,
        Armor = armor,
        Resistance = resistance,
        MaxShield = (int)Math.Ceiling(maxHp / 3m),
        ResourceTypes = resource1 != null ? new IResourceType[] { resource1 } : Array.Empty<IResourceType>()
    };
}
