using System.Collections.Generic;

public interface MapGenerationRule3
{
    public abstract List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress);
}