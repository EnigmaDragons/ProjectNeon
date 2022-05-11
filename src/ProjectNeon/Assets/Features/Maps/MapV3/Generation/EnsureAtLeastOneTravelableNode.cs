using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastOneTravelableNode : MapGenerationRule3
{
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (list.All(x => x.PreventTravel))
            list.Add(new MapNode3 { Type = MapNodeType.Combat });
        return list;
    }
}
