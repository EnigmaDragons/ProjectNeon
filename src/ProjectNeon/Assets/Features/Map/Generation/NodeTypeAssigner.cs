using System;
using System.Collections.Generic;
using System.Linq;

public class NodeTypeAssigner
{
    private readonly AdventureProgress2 _progress;
    private readonly List<MapGenerationRule> _generationRules = new List<MapGenerationRule> { new NoClinicWithinEarlyColumns(), new GarunteedClinicBeforeBoss() };

    public NodeTypeAssigner(AdventureProgress2 progress)
    {
        _progress = progress;
    }
    
    public void Assign(List<List<MapNode>> map)
    {
        var totalNodes = map.SelectMany(x => x).Count() - 2;
        var possibilities = _progress.CurrentStage.NodeTypeOdds.GenerateFreshSet();
        possibilities = Enumerable.Range(0, (int)Math.Ceiling((decimal)totalNodes / possibilities.Count)).SelectMany(x => possibilities).ToList();
        var nodeTypes = possibilities.Distinct().ToArray();
        for (var column = map.Count - 2; column > 0; column--)
        {
            foreach (var node in map[column])
            {
                var possibleNodeTypes = nodeTypes.ToList();
                foreach (var rule in _generationRules)
                    possibleNodeTypes = rule.FilterNodeTypes(possibleNodeTypes, map, node, column);
                var nodePossibilities = possibilities.Where(possibility => possibleNodeTypes.Any(nodeType => nodeType == possibility)).ToArray();
                if (nodePossibilities.Any())
                {
                    var nodeType = nodePossibilities.Random();
                    node.Type = nodeType;
                    possibilities.Remove(nodeType);
                }
                else
                    node.Type = possibleNodeTypes.Random();
            }
        }
    }
}