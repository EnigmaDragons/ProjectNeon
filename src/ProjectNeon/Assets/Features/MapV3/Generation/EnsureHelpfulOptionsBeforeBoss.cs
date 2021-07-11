using System.Collections.Generic;
using System.Linq;

public class EnsureHelpfulOptionsBeforeBoss : MapGenerationRule3
{
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (map.Progress + 2 == progress.CurrentChapter.SegmentCount)
            return new List<MapNode3> { new MapNode3 { Type = MapNodeType.Clinic }, new MapNode3 { Type = MapNodeType.CardShop }, new MapNode3 { Type = MapNodeType.GearShop } };
        return list;
    }
}