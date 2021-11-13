using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EquipmentShop")]
public class EquipmentShopSegment : StageSegment
{
    public override string Name => "Equipment Shop";
    public override void Start() => Message.Publish(new ToggleEquipmentShop());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => new GeneratedEquipmentShopSegment(mapData.Corp);
}