using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EliteCombat")]
public class EliteCombatSegment : StageSegment
{
    public override string Name => "Elite Combat";
    public override void Start() => Message.Publish(new EnterRandomEliteCombat());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx)
        => new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentStage.Battleground, true,
            ctx.Adventure.CurrentStage.EliteEncounterBuilder.Generate(ctx.Adventure.CurrentElitePowerLevel).ToArray());
}