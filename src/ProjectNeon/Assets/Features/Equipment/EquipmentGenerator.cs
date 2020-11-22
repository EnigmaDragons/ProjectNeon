using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentGenerator
{
    private static readonly string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-";

    private static readonly Dictionary<EquipmentSlot, string[]> _slotNames = new Dictionary<EquipmentSlot, string[]>
    {
        { EquipmentSlot.Weapon, new [] {
            "Blaster",
            "Zapper",
            "HandLaz",
            "Rifle",
            "Shooter"
        }},
        { EquipmentSlot.Armor, new []{
            "Power Suit",
            "ExoComp",
            "Cuirass",
            "Gauntlet",
            "Kevlar Jacket",
            "Flak Gear",
            "Linear Frame"
        }},
        { EquipmentSlot.Augmentation, new[] {
            "Booster",
            "Enhancer",
            "Stimulator",
        }}
    };
   
    private static readonly StatType[] GeneratableStats = { StatType.MaxHP, StatType.Attack, StatType.Magic, StatType.Armor, StatType.Toughness, StatType.Resistance };
    
    public Equipment GenerateRandomCommon()
        => Generate(new [] { 1, 2 }.Random(), Rarity.Common);
    
    public Equipment GenerateRandomUncommon() 
        => Generate(new [] { 3, 4, 5 }.Random(), Rarity.Uncommon);
    
    public Equipment GenerateRandomRare() 
        => Generate(new [] { 5, 6, 7 }.Random(), Rarity.Rare);

    private static readonly Dictionary<Rarity, int[]> RarityPowers = new Dictionary<Rarity, int[]>
    {
        { Rarity.Common, new []{1,2}},
        { Rarity.Uncommon, new []{3, 4, 5}},
        { Rarity.Rare, new [] {5, 6, 7}}
    };

    private int PowerLevelFor(Rarity rarity)
        => RarityPowers[rarity].Random();
    
    private EquipmentSlot SlotFor(StatType primaryStatType)
        => new[] {StatType.Attack, StatType.Magic}.Contains(primaryStatType)
            ? EquipmentSlot.Weapon
            : new[] {StatType.Armor, StatType.Resistance}.Contains(primaryStatType)
                ? EquipmentSlot.Armor
                : EquipmentSlot.Augmentation;

    private StatType RandomStatFor(EquipmentSlot slot)
        => slot == EquipmentSlot.Weapon
            ? new[] {StatType.Attack, StatType.Magic}.Random()
            : slot == EquipmentSlot.Armor
                ? new[] {StatType.Armor, StatType.Resistance}.Random()
                : new[] {StatType.Toughness, StatType.MaxHP, StatType.Leadership}.Random();
    
    public static string NameFor(EquipmentSlot slot, Rarity rarity)
        => $"{string.Join("", Enumerable.Range(0, Rng.Int(3, 6)).Select(_ => Letters.Random()))} {_slotNames[slot].Random()}";

    // TODO: Refactor out these two generate methods
    private Equipment Generate(int totalPowerLevel, Rarity rarity)
    {
        var description = "";
        var modifiers = new List<EquipmentStatModifier>();
        
        var selectedStats = new HashSet<string>();
        var primarySelectedStat = GeneratableStats.Random();
        selectedStats.Add(primarySelectedStat.ToString());
        var slot = SlotFor(primarySelectedStat);
        var name = NameFor(slot, rarity);
        
        var remainingPointsToSpend = totalPowerLevel;
        var selectedStat = primarySelectedStat;
        while (remainingPointsToSpend > 0)
        {
            var attributePointsToSpend = Rng.Int(1, remainingPointsToSpend + 1);
            remainingPointsToSpend -= attributePointsToSpend;

            modifiers.Add(AdditiveModifier(attributePointsToSpend, selectedStat));
            description += modifiers.Last().Describe();
            
            if (remainingPointsToSpend > 0)
            {
                description += " ";
                selectedStat = GeneratableStats.Where(s => !selectedStats.Contains(s.ToString())).Random();
                selectedStats.Add(selectedStat.ToString());
            }
        }
        
        return new InMemoryEquipment
        {
            Name = name,
            Rarity = rarity,
            Price = (totalPowerLevel * 42).WithShopPricingVariance(),
            Classes = new [] { CharacterClass.All },
            Description = description,
            Modifiers = modifiers.ToArray(),
            Slot = slot
        };
    }
    
    
    public Equipment Generate(Rarity rarity, EquipmentSlot slot)
    {
        var description = "";
        var modifiers = new List<EquipmentStatModifier>();
        
        var selectedStats = new HashSet<string>();
        var primarySelectedStat = RandomStatFor(slot);
        selectedStats.Add(primarySelectedStat.ToString());
        var name = NameFor(slot, rarity);

        var totalPowerLevel = PowerLevelFor(rarity);
        var remainingPointsToSpend = totalPowerLevel;
        var selectedStat = primarySelectedStat;
        while (remainingPointsToSpend > 0)
        {
            var attributePointsToSpend = Rng.Int(1, remainingPointsToSpend + 1);
            remainingPointsToSpend -= attributePointsToSpend;

            modifiers.Add(AdditiveModifier(attributePointsToSpend, selectedStat));
            description += modifiers.Last().Describe();
            
            if (remainingPointsToSpend > 0)
            {
                description += " ";
                selectedStat = GeneratableStats.Where(s => !selectedStats.Contains(s.ToString())).Random();
                selectedStats.Add(selectedStat.ToString());
            }
        }
        
        return new InMemoryEquipment
        {
            Name = name,
            Rarity = rarity,
            Price = (totalPowerLevel * 42).WithShopPricingVariance(),
            Classes = new [] { CharacterClass.All },
            Description = description,
            Modifiers = modifiers.ToArray(),
            Slot = slot
        };
    }

    private EquipmentStatModifier AdditiveModifier(int powerLevel, StatType stat)
    {
        var rawAmount = AdditiveStatsChartPerPoint[stat] * powerLevel;
        return new EquipmentStatModifier
        {
            StatType = stat.ToString(),
            ModifierType = StatMathOperator.Additive,
            Amount = Mathf.Max(rawAmount, 1)
        };
    }

    private static readonly Dictionary<StatType, int> AdditiveStatsChartPerPoint = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 3 },
        { StatType.Armor, 2 },
        { StatType.Resistance, 2 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Toughness, 2 },
        { StatType.Leadership, 1 }
    };
}
