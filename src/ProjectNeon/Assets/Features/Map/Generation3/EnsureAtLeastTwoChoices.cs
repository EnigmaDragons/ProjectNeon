using System.Collections.Generic;
using System.Linq;

public class EnsureAtLeastTwoChoices : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        while (list.Count < 2)
        {
            if (map.CompletedNodes.Count + 2 == map.TotalNodeCount && !list.Any(x => x == MapNodeType.StoryEvent))
                list.Add(MapNodeType.StoryEvent);
            else
                list.Add(MapNodeType.Combat);
        }
        return list;
    }
}