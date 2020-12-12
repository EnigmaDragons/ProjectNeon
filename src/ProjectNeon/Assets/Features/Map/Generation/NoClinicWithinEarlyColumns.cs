
using System.Collections.Generic;

public class NoClinicWithinEarlyColumns : MapGenerationRule
{
    public override bool IsValid(MapNodeType m, int column, List<List<MapNode>> currentMap)
    {
        return m != MapNodeType.Clinic || column >= 3;
    }
}
