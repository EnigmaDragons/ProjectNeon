using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/NodeOdds")]
public class NodeTypeOdds : ScriptableObject
{
    [SerializeField] private int combatChances;
    [SerializeField] private int eliteCombatChances;
    [SerializeField] private int cardShopChances;
    [SerializeField] private int gearShopChances;
    [SerializeField] private int storyEventChances;
    [SerializeField] private int clinicChances;

    public MapNodeType GetRandomNodeType()
    {
        var roll = Rng.Int(0, combatChances + eliteCombatChances + cardShopChances + gearShopChances + storyEventChances + clinicChances);
        var nodeTable = NumberOf(MapNodeType.Combat, combatChances)
            .Concat(NumberOf(MapNodeType.Elite, eliteCombatChances))
            .Concat(NumberOf(MapNodeType.CardShop, cardShopChances))
            .Concat(NumberOf(MapNodeType.GearShop, gearShopChances))
            .Concat(NumberOf(MapNodeType.StoryEvent, storyEventChances))
            .Concat(NumberOf(MapNodeType.Clinic, clinicChances)).ToArray();
        return nodeTable[roll];

//        if (roll < combatChances)
//            return MapNodeType.Combat;
//        if (roll < combatChances + eliteCombatChances)
//            return MapNodeType.Elite;
//        if (roll < combatChances + eliteCombatChances + cardShopChances)
//            return MapNodeType.CardShop;
//        if (roll < combatChances + eliteCombatChances + cardShopChances + gearShopChances)
//            return MapNodeType.GearShop;
//        if (roll < combatChances + eliteCombatChances + cardShopChances + gearShopChances + storyEventChances)
//            return MapNodeType.StoryEvent;
//        return MapNodeType.Clinic;
    }

    private IEnumerable<MapNodeType> NumberOf(MapNodeType type, int n) 
        => Enumerable.Range(0, n).Select(x => type);
}