using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EquipmentDistributionRules
{
    public StatType[] RequiresStatType = new StatType[0];
    public StatType[] ExcludeIfPartyHasStatType = new StatType[0];

    public bool ShouldInclude(StatType primaryStat) => ShouldInclude(new HashSet<StatType> {primaryStat});
    public bool ShouldInclude(HashSet<StatType> primaryStats)
    {
        return (RequiresStatType.None() || RequiresStatType.All(primaryStats.Contains))
               && (ExcludeIfPartyHasStatType.None() || ExcludeIfPartyHasStatType.None(primaryStats.Contains));
    }
}
