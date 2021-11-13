using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Boss")]
public class BossSegment : StageSegment
{
    public override string Name => "Boss Battle";
    public override void Start() => Message.Publish(new EnterBossBattle());
    public override Maybe<string> Detail => Maybe<string>.Missing();

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData)
        => new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.BossBattlefield, false, ctx.Adventure.CurrentChapter.BossEnemies.Select(x => x.ForStage(ctx.Adventure.CurrentChapterNumber)).ToArray());
}