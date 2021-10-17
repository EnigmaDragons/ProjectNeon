using System.Collections.Generic;
using System.Linq;

public class NoClinicsIfYouAreHighHealth : MapGenerationRule3
{
    public List<MapNode3> Apply(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (!party.Heroes.Any(hero => hero.Health.InjuryCounts.Any(injury => injury.Value > 0) || hero.Health.MissingHp > 10) && list.Any(x => x.Type == MapNodeType.Clinic))
            list.Remove(list.First(x => x.Type == MapNodeType.Clinic));
        return list;
    }
}