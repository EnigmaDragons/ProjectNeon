using System.Collections.Generic;
using System.Linq;

public class NoClinicsIfYouAreHighHealth : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (!party.Heroes.Any(hero => hero.Health.InjuryCounts.Any(injury => injury.Value > 0) || hero.Health.MissingHp > 10))
            list.Remove(MapNodeType.Clinic);
        return list;
    }
}