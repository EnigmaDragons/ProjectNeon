using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Boss")]
public class BossSegment : StageSegment
{
    public override string Name => "Boss Battle";
    public override void Start() => Message.Publish(new EnterBossBattle());
    public override Maybe<string> Detail => Maybe<string>.Missing();

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData)
        => new GeneratedBattleStageSegment(Name, ctx.BossDetails.Battlefield, false, ctx.BossDetails.Enemies.Select(x => x.ForStage(ctx.BossDetails.CurrentChapterNumber)).ToArray());
}