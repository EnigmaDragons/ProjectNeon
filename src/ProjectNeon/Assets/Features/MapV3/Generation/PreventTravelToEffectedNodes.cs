using System;
using System.Collections.Generic;

public class PreventTravelToEffectedNodes : MapGenerationRule3
{
    private readonly Tuple<string, MapNodeType>[] _prevented;
    
    public PreventTravelToEffectedNodes(Tuple<string, MapNodeType>[] prevented)
    {
        _prevented = prevented;
    }

    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        list.ForEach(l =>
        {
            _prevented.ForEach(p =>
            {
                var corp = p.Item1;
                var nodeType = p.Item2;
                if (!l.PreventTravel && l.Type == nodeType && l.Corp.Equals(corp))
                {
                    l.PreventTravel = true;
                    DevLog.Info($"Travel Prevented to {corp} {nodeType}");
                }
            });
        });
        return list;
    }
}