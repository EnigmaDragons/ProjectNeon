using System.Collections.Generic;
using System.Linq;

public class OnlyBossOnFinalNode : MapGenerationRule3
{
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (map.Progress >= progress.CurrentChapter.SegmentCount)
            return new List<MapNode3> { new MapNode3 { Type = MapNodeType.Boss }};
        var boss = list.FirstOrMaybe(x => x.Type == MapNodeType.Boss);
        boss.IfPresent(x => list.Remove(x));
        return list;
    }
}