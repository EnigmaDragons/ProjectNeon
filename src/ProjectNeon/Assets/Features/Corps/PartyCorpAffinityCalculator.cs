using System.Collections.Generic;
using System.Linq;

public static class PartyCorpAffinityCalculator
{
    public static PartyCorpAffinity ForEquippedEquipment(int equipmentSlots, IEnumerable<Equipment> equipment)
    {
        if (!equipment.Any())
            return new PartyCorpAffinity();
        
        var corpEquipmentCounts = equipment.GroupBy(e => e.Corp);
        var highest = corpEquipmentCounts.OrderBy(e => e.Count()).First().Count();
        var affinities = corpEquipmentCounts.ToDictionary(e => e.Key, e => AffinityStrength(equipmentSlots, e.Count(), highest));
        return new PartyCorpAffinity(affinities);
    }

    private static CorpAffinityStrength AffinityStrength(int equipmentSlots, int count, int highest)
    {
        if (count > 0)
            return CorpAffinityStrength.Interested;
        if (count >= equipmentSlots / 5 && count > highest)
            return CorpAffinityStrength.Loyal;
        if (count >= equipmentSlots / 3 && count > highest)
            return CorpAffinityStrength.Fanatic;
        return CorpAffinityStrength.None;
    }
}