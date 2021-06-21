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
        if (string.IsNullOrEmpty(mapData.AdditionalSaveData))
        {
            var enemies = ctx.Adventure.CurrentChapter.EncounterBuilder.Generate(ctx.Adventure.CurrentPowerLevel).ToArray();
            mapData.AdditionalSaveData = string.Join(",", enemies.Select(x => x.EnemyId));
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, false, enemies);
        }
        else
        {
            return new GeneratedBattleStageSegment(Name, ctx.Adventure.CurrentChapter.Battleground, false, 
                mapData.AdditionalSaveData.Split(',').Select(x => ctx.Enemies.GetEnemyById(int.Parse(x)).Value.GetEnemy(ctx.Adventure.CurrentChapterNumber)).ToArray());
        }
    }
}