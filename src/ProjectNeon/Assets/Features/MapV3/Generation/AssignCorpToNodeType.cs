using System.Collections.Generic;
using System.Linq;

public class AssignCorpToNodeType : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly Corp[] _corps;

    public AssignCorpToNodeType(MapNodeType mapNodeType, Corp[] corps)
    {
        _mapNodeType = mapNodeType;
        _corps = corps;
    }
    
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var nodeCount = list.Count(x => x.Type == _mapNodeType);
        var corps = _corps.Shuffled().Take(nodeCount).ToArray();
        while (corps.Length < nodeCount)
        {
            list.Remove(list.First(x => x.Type == _mapNodeType));
            nodeCount--;
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