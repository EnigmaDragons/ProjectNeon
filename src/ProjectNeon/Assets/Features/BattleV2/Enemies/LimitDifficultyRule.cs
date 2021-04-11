using System.Linq;

public class LimitDifficultyRule : EncounterBuildingRule
{
    private readonly float _flexibility;

    public LimitDifficultyRule(float flexibility)
        => _flexibility = flexibility;
    
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
    {
        if (!ctx.PossibleEnemies.Any())
            return ctx;
        
        var maximum = ctx.TargetDifficulty * (1 + _flexibility) - ctx.CurrentDifficulty;
        
        if (ctx.PossibleEnemies.Any(e => e.PowerLevel <= maximum))
            return ctx.WithPossibilities(e => e.PowerLevel <= maximum);
        
        var difficultyDeficit = ctx.TargetDifficulty - ctx.CurrentDifficulty;
        var minDifficultyExcess = ctx.CurrentDifficulty + ctx.PossibleEnemies.OrderBy(x => x.PowerLevel).First().PowerLevel - ctx.TargetDifficulty;
        if (difficultyDeficit > minDifficultyExcess || ctx.SelectedEnemies.Length == 0)
            return ctx.WithPossibilities(new [] { ctx.PossibleEnemies.OrderBy(x => x.PowerLevel).First() });
        
        return ctx.WithPossibilities(new EnemyInstance[0]);
    }
}