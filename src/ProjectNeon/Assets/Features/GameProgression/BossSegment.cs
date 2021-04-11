using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Boss")]
public class BossSegment : StageSegment
{
    public override string Name => "Boss Battle";
    public override void Start() => Message.Publish(new EnterBossBattle());
    public override Maybe<string> Detail => Maybe<string>.Missing();

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx)
        => new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentStage.BossBattlefield, false, ctx.Adventure.CurrentStage.BossEnemies.Select(x => x.GetEnemy(ctx.Adventure.Stage)).ToArray());
}