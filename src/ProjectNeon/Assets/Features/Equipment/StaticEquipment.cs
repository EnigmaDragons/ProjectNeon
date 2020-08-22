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

    public string Name => displayName;
    public string Description => description;
    public int Price => cost;
    public Rarity Rarity => rarity;
    public CharacterClass[] Classes => canUseClasses;
    public EquipmentSlot Slot => slot;
    
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
