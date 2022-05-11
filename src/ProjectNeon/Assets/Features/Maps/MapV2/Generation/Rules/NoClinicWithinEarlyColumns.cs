
using System.Collections.Generic;
using System.Linq;

public class NoClinicWithinEarlyColumns : MapGenerationRule
{
    public override List<MapNodeType> FilterNodeTypes(List<MapNodeType> remainingTypes, List<List<MapNode>> map, MapNode node, int column, List<MapNode> toNodes)
        => column >= 3 ? remainingTypes : remainingTypes.Where(x => x != MapNodeType.Clinic).ToList();
}
