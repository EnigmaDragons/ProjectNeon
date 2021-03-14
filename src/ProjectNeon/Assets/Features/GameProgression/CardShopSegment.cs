using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/CardShop")]
public class CardShopSegment : StageSegment
{
    public override string Name => "Card Shop";
    public override void Start() => Message.Publish(new ToggleCardShop());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx) => this;
}
