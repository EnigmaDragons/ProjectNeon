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
        if (string.IsNullOrEmpty(mapData.AdditionalSaveData))
        {
            var enemies = ctx.Adventure.CurrentChapter.EliteEncounterBuilder.Generate(ctx.Adventure.CurrentElitePowerLevel).ToArray();
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