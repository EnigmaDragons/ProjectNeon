using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class InMemoryEquipment : Equipment
{
    [SerializeField] private int id = -1;
    [SerializeField] private string name = "None";
    [SerializeField] private string description = "None";
    [SerializeField] private int price = 0;
    [SerializeField] private Rarity rarity = Rarity.Basic;
    [SerializeField] private string[] archetypes = new string[0];
    [SerializeField] private EquipmentSlot slot = EquipmentSlot.Weapon;
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[0];
    [SerializeField] private SerializableResourceType[] resourceModifiers = new SerializableResourceType[0];
    [SerializeField] private EffectData[] turnStartEffects = new EffectData[0];
    [SerializeField] private EffectData[] turnEndEffects = new EffectData[0];
    [SerializeField] private EffectData[] battleStartEffects = new EffectData[0];
    [SerializeField] private string corp = "None";
    
    // Necessary for Save Load System
    public int Id
    {
        get => id;
        set => id = value;
    }
    
    public string Name
    {
        set => name = value;
        get => name;
    }

    public string Description
    {
        set => description = value;
        get => description;
    }

    public int Price
    {
        set => price = value;
        get => price;
    }

    public Rarity Rarity
    {
        set => rarity = value;
        get => rarity;
    }

    public string[] Archetypes
    {
        set => archetypes = value;
        get => archetypes;
    }

    public EquipmentSlot Slot
    {
        set => slot = value;
        get => slot;
    }

    public EquipmentStatModifier[] Modifiers
    {
        get => modifiers;
        set => modifiers = value;
    }

    public IResourceType[] ResourceModifiers
    {
        get => resourceModifiers.Cast<IResourceType>().ToArray();
        set => resourceModifiers = value.Select(x => new SerializableResourceType(x)).ToArray();
    }

    public EffectData[] TurnStartEffects
    {
        get => turnStartEffects;
        set => turnStartEffects = value;
    }

    public EffectData[] TurnEndEffects
    {
        get => turnEndEffects;
        set => turnEndEffects = value;
    }

    public EffectData[] BattleStartEffects
    {
        get => battleStartEffects;
        set => battleStartEffects = value;
    }

    public string Corp
    {
        get => corp;
        set => corp = value;
    }

    public GameEquipmentData GetData() 
        => new GameEquipmentData { Type = GameEquipmentDataType.GeneratedEquipment, GeneratedEquipment = this };

    public Equipment Initialized(string archetype)
    {
        Archetypes = new[] {archetype};
        return this;
    }
}
