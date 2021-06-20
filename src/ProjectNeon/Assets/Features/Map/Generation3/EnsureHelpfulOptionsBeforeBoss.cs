using System.Collections.Generic;
using System.Linq;

public class EnsureHelpfulOptionsBeforeBoss : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        if (map.CompletedNodes.Count + 2 == map.TotalNodeCount)
            return new List<MapNodeType> { MapNodeType.Clinic, MapNodeType.CardShop, MapNodeType.GearShop };
        return list;
    }
}