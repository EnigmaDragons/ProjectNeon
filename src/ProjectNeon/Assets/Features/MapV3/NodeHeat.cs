using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Node Heat")]
public class NodeHeat : ScriptableObject
{
    [SerializeField] private int combatHeat = 1;
    [SerializeField] private int eliteHeat = 1;
    [SerializeField] private int cardShopHeat = 1;
    [SerializeField] private int gearShopHeat = 1;
    [SerializeField] private int clinicHeat = 1;

    public int Heat(MapNodeType nodeType)
    {
        return nodeType switch
        {
            MapNodeType.Unknown => 0,
            MapNodeType.Start => 0,
            MapNodeType.Combat => combatHeat,
            MapNodeType.Boss => 0,
            MapNodeType.Elite => eliteHeat,
            MapNodeType.CardShop => cardShopHeat,
            MapNodeType.GearShop => gearShopHeat,
            MapNodeType.StoryEvent => 0,
            MapNodeType.Clinic => clinicHeat,
        };
    }
}