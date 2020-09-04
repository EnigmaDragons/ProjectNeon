using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Equipment")]
public class StaticEquipment : ScriptableObject, Equipment
{
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;
    [SerializeField] private CharacterClass[] canUseClasses = new CharacterClass[1];
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[1];
    [SerializeField] private ResourceTypeModifications[] resourceModifiers = new ResourceTypeModifications[0];
    [SerializeField] private EffectData[] turnStartEffects = new EffectData[0];
    [SerializeField] private EffectData[] turnEndEffects = new EffectData[0];
    [SerializeField] private EffectData[] battleStartEffects = new EffectData[0];
    
    public string Name => displayName;
    public string Description => description;
    public int Price => cost;
    public Rarity Rarity => rarity;
    public string[] Classes => canUseClasses.Select(c => c.Name).ToArray();
    public EquipmentSlot Slot => slot;
    public EquipmentStatModifier[] Modifiers => modifiers.ToArray();
    public IResourceType[] ResourceModifiers => resourceModifiers;
    public EffectData[] TurnStartEffects => turnStartEffects;
    public EffectData[] TurnEndEffects => turnEndEffects;
    public EffectData[] BattleStartEffects => battleStartEffects;

    public IStats AdditiveStats()
    {
        var stats = new StatAddends();
        modifiers.Where(x => x.ModifierType == StatMathOperator.Additive)
            .ForEach(m => stats.WithRaw(m.StatType, m.Amount));
        return stats;
    }

    public StaticEquipment Initialized(CharacterClass characterClass)
    {
        canUseClasses = new[] {characterClass};
        return this;
    }
}
