using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Combat")]
public class CombatSegment : StageSegment
{
    public override string Name => "Battle";
    public override void Start() => Message.Publish(new EnterRandomCombat());
    public override Maybe<string> Detail => Maybe<string>.Missing();

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData)
    {
        if (mapData.EnemyIds == null || !mapData.EnemyIds.Any())
        {
            var enemies = ctx.Adventure.CurrentChapter.EncounterBuilder.Generate(ctx.Adventure.CurrentPowerLevel).ToArray();
            mapData.EnemyIds = enemies.Select(x => x.EnemyId).ToArray();
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, false, enemies);
        }
        else
        {
            var maybeEnemies = mapData.EnemyIds.Select(x => ctx.Enemies.GetEnemyById(x)).ToArray();
            if (maybeEnemies.Any(x => x.IsMissing))
            {
                mapData.EnemyIds = new int[0];
                return GenerateDeterministic(ctx, mapData);
            }
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, false, 
                maybeEnemies.Select(x => x.Value.GetEnemy(ctx.Adventure.CurrentChapterNumber)).ToArray());
        }
    }
}