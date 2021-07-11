using System.Collections.Generic;
using System.Linq;

public class EnsureNodeTypeHasCorp : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly Corp[] _gearCorps;

    public EnsureNodeTypeHasCorp(MapNodeType mapNodeType, Corp[] gearCorps)
    {
        _mapNodeType = mapNodeType;
        _gearCorps = gearCorps;
    }
    
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var gearShopCount = list.Count(x => x.Type == _mapNodeType);
        var corps = _gearCorps.Shuffled().Take(gearShopCount).ToArray();
        while (corps.Length < gearShopCount)
        {
            list.Remove(list.First(x => x.Type == _mapNodeType));
            gearShopCount--;
        }
        var i = 0;
        foreach (var node in list)
        {
            if (node.Type == _mapNodeType)
            {
                node.Corp = corps[i].Name;
                i++;
            }
        }
        return list;
    }
}