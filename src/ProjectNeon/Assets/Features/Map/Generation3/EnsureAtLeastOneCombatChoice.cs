using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastOneCombatChoice : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        if (!list.Any(x => x == MapNodeType.Combat || x == MapNodeType.Elite))
            list.Add(MapNodeType.Combat);
        return list;
    }
}