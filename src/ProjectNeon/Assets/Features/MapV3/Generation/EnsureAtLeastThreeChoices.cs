using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastThreeChoices : MapGenerationRule3
{
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        while (list.Count < 3)
            list.Add(new MapNode3 { Type = MapNodeType.Combat });
        return list;
    }
}