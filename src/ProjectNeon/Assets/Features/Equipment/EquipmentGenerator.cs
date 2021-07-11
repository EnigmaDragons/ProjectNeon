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
            "Shooter",
            "Banner"
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
    };
   
    private static readonly StatType[] GeneratableStats =
    {
        StatType.MaxHP, 
        StatType.MaxShield, 
        StatType.StartingShield, 
        StatType.Attack, 
        StatType.Magic, 
        StatType.Armor, 
        StatType.Toughness, 
        StatType.Resistance, 
        StatType.Leadership
    };

    private readonly Dictionary<StatType, string> CorpStats = new Dictionary<StatType, string>();

    public EquipmentGenerator(AllCorps corps)
    {
        foreach (var corp in corps.GetCorps())
            foreach (var stat in corp.GeneratedEquipmentPrimaryStatPreference)
                CorpStats[stat] = corp.Name;
    }

    public Equipment GenerateRandomCommon()
        => Generate(RarityPowers[Rarity.Common].Random(), Rarity.Common);
    
    public Equipment GenerateRandomUncommon() 
        => Generate(RarityPowers[Rarity.Uncommon].Random(), Rarity.Uncommon);
    
    public Equipment GenerateRandomRare() 
        => Generate(RarityPowers[Rarity.Rare].Random(), Rarity.Rare);
    
    public Equipment GenerateRandomEpic()
        => Generate(RarityPowers[Rarity.Epic].Random(), Rarity.Epic);

    private static readonly Dictionary<Rarity, int[]> RarityPowers = new Dictionary<Rarity, int[]>
    {
        { Rarity.Common, new []{1, 2}},
        { Rarity.Uncommon, new []{2, 3}},
        { Rarity.Rare, new [] {4, 5, 6}},
        { Rarity.Epic, new [] {6, 7, 8}}
    };

    private int PowerLevelFor(Rarity rarity)
        => RarityPowers[rarity].Random();

    private EquipmentSlot SlotFor(StatType primaryStatType)
        => new[] {StatType.Attack, StatType.Magic, StatType.Leadership}.Contains(primaryStatType)
            ? EquipmentSlot.Weapon
            : EquipmentSlot.Armor;

    private StatType RandomStatFor(EquipmentSlot slot)
        => slot == EquipmentSlot.Weapon
            ? new[] { 
                StatType.Attack, StatType.Attack, 
                StatType.Magic, StatType.Magic, 
                StatType.Leadership, StatType.Leadership, 
                StatType.Toughness 
            }.Random()
            : new[]
            {
                StatType.StartingShield, StatType.StartingShield,
                StatType.Armor, StatType.Armor, 
                StatType.Resistance, StatType.Resistance, 
                StatType.MaxHP, StatType.MaxHP, 
                StatType.MaxShield
            }.Random();
    
    public static string NameFor(EquipmentSlot slot, Rarity rarity)
        => $"{string.Join("", Enumerable.Range(0, Rng.Int(3, 6)).Select(_ => Letters.Random()))} {_slotNames[slot].Random()}";

    private Equipment Generate(int totalPowerLevel, Rarity rarity)
    {
        var description = "";
        var modifiers = new List<EquipmentStatModifier>();
        
        var selectedStats = new HashSet<string>();
        var slot = new[] {EquipmentSlot.Weapon, EquipmentSlot.Armor}.Random();
        var primarySelectedStat = RandomStatFor(slot);
        selectedStats.Add(primarySelectedStat.ToString());
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
            Price = CardShopPricing.EquipmentShopPrice(rarity, 1f),
            Archetypes = new string[0],
            Description = description,
            Modifiers = modifiers.ToArray(),
            Slot = slot,
            Corp = CorpStats[primarySelectedStat]
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
        { StatType.MaxHP, 4 },
        { StatType.MaxShield, 5 },
        { StatType.StartingShield, 3 },
        { StatType.Armor, 1 },
        { StatType.Resistance, 1 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Toughness, 1 },
        { StatType.Leadership, 1 }
    };
}
