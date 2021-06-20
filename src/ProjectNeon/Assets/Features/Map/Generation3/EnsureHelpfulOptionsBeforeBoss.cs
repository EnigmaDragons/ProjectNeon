using System.Collections.Generic;
using System.Linq;

public class EnsureHelpfulOptionsBeforeBoss : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (map.Progress + 2 == progress.CurrentChapter.SegmentCount)
            return new List<MapNodeType> { MapNodeType.Clinic, MapNodeType.CardShop, MapNodeType.GearShop };
        return list;
    }
}