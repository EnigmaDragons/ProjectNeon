using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Adventure/ShopSegment")]
public class ShopSegment : StageSegment
{
    public override string Name => "Shop";
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override void Start() => SceneManager.LoadScene("ShopScene");
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
