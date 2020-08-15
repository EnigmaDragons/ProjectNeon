using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Equipment : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;
    [SerializeField] private CharacterClass[] canUseClasses = new CharacterClass[1];
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[1];

    public IStats AdditiveStats()
    {
        var stats = new StatAddends();
        modifiers.Where(x => x.ModifierType == StatMathOperator.Additive)
            .ForEach(m => stats.WithRaw(m.StatType, m.Amount));
        return stats;
    }
}
