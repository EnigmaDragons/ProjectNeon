
using System.Collections.Generic;

public abstract  class MapGenerationRule
{
    public abstract bool IsValid(MapNodeType m, int column, List<List<MapNode>> currentMap);
}
