using System.Collections.Generic;
using System.Linq;

public class EnsureGearShopsHaveCorps : MapGenerationRule3
{
    private readonly Corp[] _gearCorps;

    public EnsureGearShopsHaveCorps(Corp[] gearCorps) => _gearCorps = gearCorps;
    
    public List<MapNode3> FilterNodeTypes(List<MapNode3> list, CurrentGameMap3 map, PartyAdventureState party, AdventureProgress2 progress)
    {
        var gearShopCount = list.Count(x => x.Type == MapNodeType.GearShop);
        var corps = _gearCorps.Shuffled().Take(gearShopCount).ToArray();
        while (corps.Length < gearShopCount)
        {
            list.Remove(list.First(x => x.Type == MapNodeType.GearShop));
            gearShopCount--;
        }
        var i = 0;
        foreach (var node in list)
        {
            if (node.Type == MapNodeType.GearShop)
            {
                node.Corp = corps[i].Name;
                i++;
            }
        }
        return list;
    }
}