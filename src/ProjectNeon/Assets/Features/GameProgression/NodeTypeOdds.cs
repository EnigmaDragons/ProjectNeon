using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/NodeOdds")]
public class NodeTypeOdds : ScriptableObject
{
    [SerializeField] private int combatChances;
    [SerializeField] private int eliteCombatChances;
    [SerializeField] private int cardShopChances;
    [SerializeField] private int gearShopChances;
    [SerializeField] private int storyEventChances;

    public MapNodeType GetRandomNodeType()
    {
        var roll = Rng.Int(0, combatChances + eliteCombatChances + cardShopChances + gearShopChances + storyEventChances);
        if (roll < combatChances)
            return MapNodeType.Combat;
        if (roll < combatChances + eliteCombatChances)
            return MapNodeType.Elite;
        if (roll < combatChances + eliteCombatChances + cardShopChances)
            return MapNodeType.CardShop;
        if (roll < combatChances + eliteCombatChances + cardShopChances + gearShopChances)
            return MapNodeType.GearShop;
        return MapNodeType.StoryEvent;
    }
}