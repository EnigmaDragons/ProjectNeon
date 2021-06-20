using System.Collections.Generic;
using System.Linq;

public class OnlyBossOnFinalNode : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        if (map.CompletedNodes.Count + 1 == map.TotalNodeCount)
            return new List<MapNodeType> {MapNodeType.Boss};
        list.Remove(MapNodeType.Boss);
        return list;
    }
}