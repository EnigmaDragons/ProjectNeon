using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EquipmentShop")]
public class EquipmentShopSegment : StageSegment
{
    public override string Name => "Equipment Shop";
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldAutoStart => false;
    public override void Start() => Message.Publish(new ToggleEquipmentShop());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.GearShop;
    public override Maybe<string> Corp => Maybe<string>.Missing();
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => new GeneratedEquipmentShopSegment(mapData.Corp);
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}