using System;
using System.Collections.Generic;

public class PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly float[] _odds;

    public PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType mapNodeType, float[] odds)
    {
        _mapNodeType = mapNodeType;
        _odds = odds;
    }
    
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party)
    {
        var lastIndexOf = map.CompletedNodes.LastIndexOf(_mapNodeType);
        var turnsSinceLastTimeYouDidThisNode = lastIndexOf == -1
            ? _odds.Length - 1
            : Math.Min(_odds.Length - 1, map.CompletedNodes.Count - lastIndexOf - 1);
        if (!Rng.Chance(_odds[turnsSinceLastTimeYouDidThisNode]))
            list.Remove(_mapNodeType);
        return list;
    }
}