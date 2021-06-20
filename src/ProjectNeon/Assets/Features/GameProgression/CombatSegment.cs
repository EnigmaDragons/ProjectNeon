using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Combat")]
public class CombatSegment : StageSegment
{
    public override string Name => "Battle";
    public override void Start() => Message.Publish(new EnterRandomCombat());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx)
        => new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, false,
             ctx.Adventure.CurrentChapter.EncounterBuilder.Generate(ctx.Adventure.CurrentPowerLevel).ToArray());
}