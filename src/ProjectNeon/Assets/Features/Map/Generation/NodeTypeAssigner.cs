﻿using System;
using System.Collections.Generic;
using System.Linq;

public class NodeTypeAssigner
{
    private readonly AdventureProgress2 _progress;
    private readonly List<MapGenerationRule> _generationRules = new List<MapGenerationRule> { new NoClinicWithinEarlyColumns(), new GarunteedClinicBeforeBoss(), new NoNonCombatDuplicates() };

    public NodeTypeAssigner(AdventureProgress2 progress)
    {
        _progress = progress;
    }
    
    public void Assign(List<List<MapNode>> map)
    {
        var totalNodes = map.SelectMany(x => x).Count() - 2;
        var possibilities = _progress.CurrentChapter.NodeTypeOdds.GenerateFreshSet();
        possibilities = Enumerable.Range(0, (int)Math.Ceiling((decimal)totalNodes / possibilities.Count)).SelectMany(x => possibilities).ToList();
        var nodeTypes = possibilities.Distinct().ToArray();
        for (var column = map.Count - 2; column > 0; column--)
        {
            foreach (var node in map[column])
            {
                var toNodes = map[column + 1].Where(x => node.ChildrenIds.Contains(x.NodeId)).ToList();
                var possibleNodeTypes = nodeTypes.ToList();
                foreach (var rule in _generationRules)
                    possibleNodeTypes = rule.FilterNodeTypes(possibleNodeTypes, map, node, column, toNodes);
                var nodePossibilities = possibilities.Where(possibility => possibleNodeTypes.Any(nodeType => nodeType == possibility)).ToArray();
                if (!nodePossibilities.Any() || possibleNodeTypes.Count == 1)
                    node.Type = possibleNodeTypes.Random();
                else
                {
                    var nodeType = nodePossibilities.Random();
                    node.Type = nodeType;
                    possibilities.Remove(nodeType);
                }
            }
        }
    }
}