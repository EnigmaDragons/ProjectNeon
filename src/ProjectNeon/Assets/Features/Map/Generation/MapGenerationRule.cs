
using System.Collections.Generic;

public abstract  class MapGenerationRule
{
    public abstract List<MapNodeType> FilterNodeTypes(List<MapNodeType> remainingTypes, List<List<MapNode>> map, MapNode node, int column);
}
