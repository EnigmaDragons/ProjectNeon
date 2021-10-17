using System.Collections.Generic;
using System.Linq;

public class UseHeatUpEventMapNodeIfTriggered : MapGenerationRule3
{
    private readonly Maybe<HeatUpEventV0> _maybeHeatUpEvent;

    public UseHeatUpEventMapNodeIfTriggered(Maybe<HeatUpEventV0> maybeHeatUpEvent)
    {
        _maybeHeatUpEvent = maybeHeatUpEvent;
    }

    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (_maybeHeatUpEvent.IsPresent)
            return _maybeHeatUpEvent.Value.MapNodes
                .Select(m => new MapNode3
                {
                    Type = m.NodeType, 
                    VisitedGlobalEffectId = m.VisitedGlobalEffect?.id ?? -1,
                    UnVisitedGlobalEffectId = m.UnvisitedGlobalEffect?.id ?? -1
                })
                .ToList();
        return list;
    }
}
