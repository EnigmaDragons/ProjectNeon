using System.Collections.Generic;

public class NoShopsIfYouAreLowOnMoney : MapGenerationRule3
{
    public List<MapNodeType> FilterNodeTypes(List<MapNodeType> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        if (party.Credits < 80)
            list.Remove(MapNodeType.CardShop);
        if (party.Credits < 150)
            list.Remove(MapNodeType.GearShop);
        return list;
    }
}