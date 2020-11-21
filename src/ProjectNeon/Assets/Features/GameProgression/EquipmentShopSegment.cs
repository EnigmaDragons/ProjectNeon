using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EquipmentShop")]
public class EquipmentShopSegment : StageSegment
{
    public override string Name => "Equipment Shop";
    
    public override void Start()
    {
        Message.Publish(new ToggleEquipmentShop());
    }
}