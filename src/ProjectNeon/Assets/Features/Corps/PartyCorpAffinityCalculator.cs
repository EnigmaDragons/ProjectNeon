using System.Collections.Generic;
using System.Linq;

public static class PartyCorpAffinityCalculator
{
    public static PartyCorpAffinity ForEquippedEquipment(int equipmentSlots, IEnumerable<Equipment> equipment)
    {
        if (!equipment.Any())
            return new PartyCorpAffinity();
        
        var corpEquipmentCounts = equipment.GroupBy(e => e.Corp);
        var highestCount = corpEquipmentCounts.OrderByDescending(e => e.Count()).First().Count();
        var exclusiveHighestCorp = corpEquipmentCounts.Count(x => x.Count() == highestCount) > 1
            ? "None"
            : corpEquipmentCounts.First(x => x.Count() == highestCount).Key;
        var exclusiveHighest = corpEquipmentCounts.Where(x => x.Count() >= highestCount).Count() == 1;
        var affinities = corpEquipmentCounts.ToDictionary(e => e.Key, e => AffinityStrength(equipmentSlots, e.Count(), exclusiveHighestCorp.Equals(e.Key)));
        return new PartyCorpAffinity(affinities);
    }

    private static CorpAffinityStrength AffinityStrength(int equipmentSlots, int count, bool isExclusiveHighest)
    {
        if (count >= equipmentSlots / 3 && isExclusiveHighest)
            return CorpAffinityStrength.Fanatic;
        if (count >= equipmentSlots / 5 && isExclusiveHighest)
            return CorpAffinityStrength.Loyal;
        if (count > 0)
            return CorpAffinityStrength.Interested;
        return CorpAffinityStrength.None;
    }
}