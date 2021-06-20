using System.Collections.Generic;
using System.Linq;

public class OnlyBossOnFinalNode : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (map.Progress + 1 == progress.CurrentChapter.SegmentCount)
            return new List<MapNodeType> {MapNodeType.Boss};
        list.Remove(MapNodeType.Boss);
        return list;
    }
}