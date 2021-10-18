using System.Collections.Generic;
using System.Linq;

public class UseHeatUpEventMapNodeIfTriggered : MapGenerationRule3
{
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var maybeHeatUpEvent = progress.TriggeredHeatUpEvent;
        if (maybeHeatUpEvent.IsPresent)
            return maybeHeatUpEvent.Value.Value.MapNodes
                .Select(m => new MapNode3
                {
                    Type = m.NodeType, 
                    IsPlotNode = true,
                    VisitedGlobalEffectId = m.VisitedGlobalEffect?.id ?? -1,
                    UnVisitedGlobalEffectId = m.UnvisitedGlobalEffect?.id ?? -1
                })
                .ToList();
        return list;
    }
}
