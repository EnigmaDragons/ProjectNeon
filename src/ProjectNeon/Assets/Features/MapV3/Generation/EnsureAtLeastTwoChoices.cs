using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastTwoChoices : MapGenerationRule3
{
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        while (list.Count < 2)
        {
            if (map.Progress + 2 == progress.CurrentChapter.SegmentCount && list.All(x => x.Type != MapNodeType.StoryEvent))
                list.Add(new MapNode3 { Type = MapNodeType.StoryEvent });
            else
                list.Add(new MapNode3 { Type = MapNodeType.Combat });
        }
        return list;
    }
}