using System.Collections.Generic;

public class GarunteedClinicBeforeBoss : MapGenerationRule
{
    public override List<MapNodeType> FilterNodeTypes(List<MapNodeType> remainingTypes, List<List<MapNode>> map, MapNode node, int column)
    {
        return column == map.Count - 2 ? new List<MapNodeType>() {MapNodeType.Clinic} : remainingTypes;
    }
}