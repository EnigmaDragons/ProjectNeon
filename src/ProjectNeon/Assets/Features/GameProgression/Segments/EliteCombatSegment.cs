using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EliteCombat")]
public class EliteCombatSegment : StageSegment
{
    public override string Name => "Elite Combat";
    public override bool ShouldCountTowardsEnemyPowerLevel => true;
    public override void Start() => Message.Publish(new EnterRandomEliteCombat());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData)
        => AdventureProgress2MapNode3CombatNodeGeneration.Generate(Name, true, ctx, mapData);
}