using System.Linq;

public static class AdventureProgress2MapNode3CombatNodeGeneration
{
    private static int MaxGenerationAttempts = 3;
    
    public static IStageSegment Generate(string name, bool isElite, AdventureGenerationContext ctx, MapNode3 mapData)
    {
        var powerLevel = isElite ? ctx.Adventure.CurrentElitePowerLevel : ctx.Adventure.CurrentPowerLevel;
        var encounterBuilder = isElite ? ctx.Adventure.CurrentChapter.EliteEncounterBuilder : ctx.Adventure.CurrentChapter.EncounterBuilder;
        if (mapData.EnemyIds == null || !mapData.EnemyIds.Any())
            return GenerateEncounter(name, isElite, ctx, mapData, encounterBuilder, powerLevel);
        
        var maybeEnemies = mapData.EnemyIds.Select(x => ctx.Enemies.GetEnemyById(x)).ToArray();
        if (maybeEnemies.All(x => x.IsPresent))
            return new GeneratedBattleStageSegment(name, ctx.Adventure.CurrentChapter.Battleground, isElite, 
                maybeEnemies.Select(x => x.Value.ForStage(ctx.Adventure.CurrentChapterNumber)).ToArray());
        
        mapData.EnemyIds = new int[0];
        return Generate(name, isElite, ctx, mapData);
    }

    private static IStageSegment GenerateEncounter(string name, bool isElite, AdventureGenerationContext ctx,
        MapNode3 mapData, EncounterBuilder encounterBuilder, int powerLevel)
    {
        var generated = false;
        var enemies = new EnemyInstance[0];
        for (var i = 0; i < MaxGenerationAttempts && !generated; i++)
        {
            enemies = encounterBuilder.Generate(powerLevel, ctx.Adventure.CurrentChapterNumber).ToArray();
            var def = new EnemyEncounterDefinition(enemies.Select(e => e.EnemyId));
            if (!ctx.Encounters.Contains(def))
                generated = true;
        }

        ctx.RecordGeneratedEncounter(enemies.Select(e => e.EnemyId));
        mapData.EnemyIds = enemies.Select(x => x.EnemyId).ToArray();
        return new GeneratedBattleStageSegment(name, ctx.Adventure.CurrentChapter.Battleground, isElite, enemies);
    }
}
