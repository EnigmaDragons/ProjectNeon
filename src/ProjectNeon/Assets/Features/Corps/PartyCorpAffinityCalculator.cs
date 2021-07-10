using System;
using System.Collections.Generic;
using System.Linq;

public static class PartyCorpAffinityCalculator
{
    public static PartyCorpAffinity ForEquippedEquipment(int equipmentSlots, Dictionary<string, Corp> allCorps, IEnumerable<Equipment> equipment)
    {
        if (!equipment.Any())
            return new PartyCorpAffinity();
        
        var corpEquipmentCounts = equipment.GroupBy(e => e.Corp).ToDictionary(e => e.Key, e => e.Count());
        var allCorpEquipmentsCount = allCorps.ToDictionary(c => c.Key, c => corpEquipmentCounts.ValueOrDefault(c.Key, 0));
        var highestCount = corpEquipmentCounts.OrderByDescending(e => e.Value).First().Value;
        var exclusiveHighestCorp = corpEquipmentCounts.Count(x => x.Value == highestCount) > 1
            ? "None"
            : corpEquipmentCounts.First(x => x.Value == highestCount).Key;
        var allCorpGearTiers = allCorpEquipmentsCount.ToDictionary(c => c.Key, c => GearTier(equipmentSlots, c.Value));
        
        var baseAffinities = allCorpGearTiers.ToDictionary(
            c => c.Key, 
            c => AffinityStrength(c.Value, exclusiveHighestCorp.Equals(c.Key), allCorpGearTiers));

        var adjustedAffinitiesForRivalries = baseAffinities.ToDictionary(
            e => e.Key,
            e => AdjustedBasedOnRivalAffinity(e.Value,
                allCorps[e.Key].RivalCorpNames
                    .Select(r => baseAffinities.ValueOrDefault(r, CorpAffinityStrength.None))));

        return new PartyCorpAffinity(adjustedAffinitiesForRivalries);
    }

    private static int GearTier(int equipmentSlots, int count)
    {
        if (count >= equipmentSlots / 3)
            return 3;
        else if (count >= equipmentSlots / 5)
            return 2;
        else if (count > 0)
            return 1;
        return 0;
    }

    private static CorpAffinityStrength AffinityStrength(int gearTier, bool isExclusiveHighest, Dictionary<string, int> corpGearTiers)
    {
        if (gearTier == 3 && isExclusiveHighest)
            return CorpAffinityStrength.Fanatic;
        else if (gearTier == 2 && isExclusiveHighest)
            return CorpAffinityStrength.Loyal;
        else if (gearTier > 0 && (isExclusiveHighest || corpGearTiers.Count(c => c.Value >= gearTier) < 3)) // You can be interested in 2 Corps simultaneously, but not 3.
            return CorpAffinityStrength.Interested;
        return CorpAffinityStrength.None;
    }

    private static CorpAffinityStrength AdjustedBasedOnRivalAffinity(CorpAffinityStrength baseAffinity, 
        IEnumerable<CorpAffinityStrength> rivalBaseAffinities)
    {
        var rivalAffinityAmount = rivalBaseAffinities.Sum(r => (int)r);
        var baseTier = (int)baseAffinity;
        var adjusted = Math.Max(-3, baseTier - rivalAffinityAmount);
        return (CorpAffinityStrength)adjusted;
    }
}