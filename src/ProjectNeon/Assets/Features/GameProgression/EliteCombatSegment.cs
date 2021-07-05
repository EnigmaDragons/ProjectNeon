using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EliteCombat")]
public class EliteCombatSegment : StageSegment
{
    public override string Name => "Elite Combat";
    public override void Start() => Message.Publish(new EnterRandomEliteCombat());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData)
    {
        if (mapData.EnemyIds == null || !mapData.EnemyIds.Any())
        {
            var enemies = ctx.Adventure.CurrentChapter.EliteEncounterBuilder.Generate(ctx.Adventure.CurrentElitePowerLevel).ToArray();
            mapData.EnemyIds = enemies.Select(x => x.EnemyId).ToArray();
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, true, enemies);
        }
        else
        {
            var maybeEnemies = mapData.EnemyIds.Select(x => ctx.Enemies.GetEnemyById(x)).ToArray();
            if (maybeEnemies.Any(x => x.IsMissing))
            {
                mapData.EnemyIds = new int[0];
                return GenerateDeterministic(ctx, mapData);
            }
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, true, 
                maybeEnemies.Select(x => x.Value.ForStage(ctx.Adventure.CurrentChapterNumber)).ToArray());
        }
    }
}