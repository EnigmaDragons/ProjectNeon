using System.Collections.Generic;
using System.Linq;

public class NoShopsIfYouAreLowOnMoney : MapGenerationRule3
{
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (party.Credits < 80)
            list = list.Where(x => x.Type != MapNodeType.CardShop).ToList();
        if (party.Credits < 150)
            list = list.Where(x => x.Type != MapNodeType.GearShop).ToList();
        return list;
    }
}