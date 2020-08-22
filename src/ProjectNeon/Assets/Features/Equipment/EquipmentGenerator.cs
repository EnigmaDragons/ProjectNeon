using System.Collections.Generic;
using System.Linq;

public class EquipmentGenerator
{
    private static readonly string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-";
    
    public Equipment GenerateRandomCommon()
    {
        var attributePointsToSpend = new [] {2, 3}.Random();
        var description = "";
        var selectedStat = new[] { StatType.MaxHP, StatType.Attack, StatType.Magic, StatType.Armor, StatType.Toughness, StatType.Resistance }.Random();
        var slot = new[] {StatType.Attack, StatType.Magic}.Contains(selectedStat)
            ? EquipmentSlot.Weapon
            : new[] {StatType.Armor, StatType.Resistance}.Contains(selectedStat)
                ? EquipmentSlot.Armor
                : EquipmentSlot.Augmentation;
        var modifiers = new List<EquipmentStatModifier>();
        modifiers.Add(new EquipmentStatModifier 
        { 
            StatType = selectedStat.ToString(), 
            ModifierType = StatMathOperator.Additive, 
            Amount = (AdditiveStatsChartPerPoint[selectedStat] * attributePointsToSpend).FlooredInt()
        });
        description += modifiers[0].Describe();
        
        return new InMemoryEquipment
        {
            Name = string.Join("", Enumerable.Range(0, Rng.Int(5, 16)).Select(_ => Letters.Random())),
            Rarity = Rarity.Common,
            Price = Rng.Int(18, 32),
            Classes = new [] { CharacterClass.All },
            Description = description,
            Modifiers = modifiers.ToArray(),
            Slot = slot
        };
    }
    
    private static readonly Dictionary<StatType, float> AdditiveStatsChartPerPoint = new Dictionary<StatType, float>
    {
        { StatType.MaxHP, 5 },
        { StatType.Armor, 1 },
        { StatType.Resistance, 1 },
        { StatType.Attack, 0.5f },
        { StatType.Magic, 0.5f },
        { StatType.Toughness, 2 }
    };
}
