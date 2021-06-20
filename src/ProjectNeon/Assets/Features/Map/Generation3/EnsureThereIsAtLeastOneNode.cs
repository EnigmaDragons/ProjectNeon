using System.Collections.Generic;
using System.Linq;

public class EnsureThereIsAtLeastOneNode : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        if (list.Any())
            return list;
        return new List<MapNodeType> {MapNodeType.StoryEvent};
    }
}