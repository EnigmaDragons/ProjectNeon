using System;
using System.Collections.Generic;
using System.Linq;

public class AssignCorpToNodeType : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly Corp[] _corps;
    private readonly Func<Corp, bool> _canIncludeCorp;

    public AssignCorpToNodeType(MapNodeType mapNodeType, Corp[] corps) 
        : this(mapNodeType, corps, c => true) {}
    
    public AssignCorpToNodeType(MapNodeType mapNodeType, Corp[] corps, Func<Corp, bool> canIncludeCorp)
    {
        _mapNodeType = mapNodeType;
        _corps = corps;
        _canIncludeCorp = canIncludeCorp;
    }
    
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var nodeCount = list.Count(x => x.Type == _mapNodeType);
        
        // Applicable Clinic Corps
        var corps = _corps
            .Where(c => _canIncludeCorp(c))
            .ToArray()
            .Shuffled()
            .Take(nodeCount)
            .ToArray();
        
        // Remove Infeasible Clinics
        while (corps.Length < nodeCount)
        {
            list.Remove(list.First(x => x.Type == _mapNodeType));
            nodeCount--;
        }
        
        // Assign Corps to Clinics
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