﻿using System;
using System.Collections.Generic;
using System.Linq;

public class AdditionalNodeChoice : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly float[] _odds;
    private int _requiredExistingNodes;

    public AdditionalNodeChoice(MapNodeType mapNodeType, float[] odds, int requiredExistingNodes)
    {
        _mapNodeType = mapNodeType;
        _odds = odds;
        _requiredExistingNodes = requiredExistingNodes;
    }
    
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (list.Count(x => x == _mapNodeType) != _requiredExistingNodes)
            return list;
        var lastIndexOf = map.CompletedNodes.LastIndexOf(_mapNodeType);
        var turnsSinceLastTimeYouDidThisNode = lastIndexOf == -1
            ? _odds.Length - 1
            : Math.Min(_odds.Length - 1, map.CompletedNodes.Count - lastIndexOf - 1);
        if (Rng.Chance(_odds[turnsSinceLastTimeYouDidThisNode]))
            list.Add(_mapNodeType);
        return list;
    }
}