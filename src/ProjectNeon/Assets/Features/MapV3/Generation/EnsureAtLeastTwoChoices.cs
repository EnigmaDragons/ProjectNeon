using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastTwoChoices : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        while (list.Count < 2)
        {
            if (map.Progress + 2 == progress.CurrentChapter.SegmentCount && list.All(x => x != MapNodeType.StoryEvent))
                list.Add(MapNodeType.StoryEvent);
            else
                list.Add(MapNodeType.Combat);
        }
        return list;
    }
}