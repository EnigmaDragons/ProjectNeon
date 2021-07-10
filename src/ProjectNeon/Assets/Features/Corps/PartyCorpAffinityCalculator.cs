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
        var corpEquipmentsCount = allCorps.ToDictionary(c => c.Key, c => corpEquipmentCounts.ValueOrDefault(c.Key, 0));
        var highestCount = corpEquipmentCounts.OrderByDescending(e => e.Value).First().Value;
        var exclusiveHighestCorp = corpEquipmentCounts.Count(x => x.Value == highestCount) > 1
            ? "None"
            : corpEquipmentCounts.First(x => x.Value == highestCount).Key;
        
        var baseAffinities = corpEquipmentsCount.ToDictionary(
            e => e.Key, 
            e => AffinityStrength(equipmentSlots, e.Value, exclusiveHighestCorp.Equals(e.Key)));

        var adjustedAffinities = baseAffinities.ToDictionary(
            e => e.Key,
            e => AdjustedBasedOnRivalAffinity(e.Value,
                allCorps[e.Key].RivalCorpNames
                    .Select(r => baseAffinities.ValueOrDefault(r, CorpAffinityStrength.None))));

        return new PartyCorpAffinity(adjustedAffinities);
    }

    private static CorpAffinityStrength AffinityStrength(int equipmentSlots, int count, bool isExclusiveHighest)
    {
        if (count >= equipmentSlots / 3 && isExclusiveHighest)
            return CorpAffinityStrength.Fanatic;
        else if (count >= equipmentSlots / 5 && isExclusiveHighest)
            return CorpAffinityStrength.Loyal;
        else if (count > 0)
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