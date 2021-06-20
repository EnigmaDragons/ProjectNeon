using System.Collections.Generic;

public interface MapGenerationRule3
{
    public abstract List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress);
}