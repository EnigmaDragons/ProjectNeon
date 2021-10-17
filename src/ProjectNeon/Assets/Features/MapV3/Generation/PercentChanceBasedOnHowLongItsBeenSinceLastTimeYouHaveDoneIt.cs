using System;
using System.Collections.Generic;
using System.Linq;

public class PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt : MapGenerationRule3
{
    private readonly MapNodeType _mapNodeType;
    private readonly float[] _odds;

    public PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType mapNodeType, float[] odds)
    {
        _mapNodeType = mapNodeType;
        _odds = odds;
    }
    
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var lastIndexOf = map.CompletedNodes.ToArray().LastIndexOf(x => x.Type == _mapNodeType);
        var turnsSinceLastTimeYouDidThisNode = lastIndexOf == -1
            ? _odds.Length - 1
            : Math.Min(_odds.Length - 1, map.CompletedNodes.Count - lastIndexOf - 1);
        if (!Rng.Chance(_odds[turnsSinceLastTimeYouDidThisNode]))
            list.Remove(list.First(x => x.Type == _mapNodeType));
        return list;
    }
}