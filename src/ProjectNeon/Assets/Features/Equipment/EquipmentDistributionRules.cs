using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EquipmentDistributionRules
{
    public StatType[] RequiresStatType = new StatType[0];
    public StatType[] ExcludeIfPartyHasStatType = new StatType[0];

    public bool ShouldInclude(HashSet<StatType> partyStatTypes) => ShouldInclude(partyStatTypes, partyStatTypes);
    public bool ShouldInclude(HashSet<StatType> heroStatTypes, HashSet<StatType> partyStatTypes)
    {
        return (RequiresStatType.None() || RequiresStatType.All(heroStatTypes.Contains))
               && (ExcludeIfPartyHasStatType.None() || ExcludeIfPartyHasStatType.None(partyStatTypes.Contains));
    }
}
