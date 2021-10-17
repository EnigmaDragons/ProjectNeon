using System.Collections.Generic;
using System.Linq;

public class EnsureHelpfulOptionsBeforeBoss : MapGenerationRule3
{
    private readonly int _maxHeatGainFromANode;
    private readonly Corp[] _clinicCorps;

    public EnsureHelpfulOptionsBeforeBoss(int maxHeatGainFromANode, Corp[] clinicCorps)
    {
        _maxHeatGainFromANode = maxHeatGainFromANode;
        _clinicCorps = clinicCorps;
    }

    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (map.Progress + _maxHeatGainFromANode >= progress.CurrentChapter.SegmentCount)
            return new List<MapNode3>
                {
                    new MapNode3 { Type = MapNodeType.CardShop }, 
                    new MapNode3 { Type = MapNodeType.GearShop }
                }
                .Concat(_clinicCorps.Select(_ => new MapNode3 { Type = MapNodeType.Clinic })) // Include every type of Clinic
                .ToList();
        return list;
    }
}