using System.Linq;

public class AvoidRepeatingElites : EncounterBuildingRule
{
    private readonly EncounterBuilderHistory _history;

    public AvoidRepeatingElites(EncounterBuilderHistory history)
    {
        _history = history;
    }
    
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
    {
        if (_history == null)
            return ctx;

        var historyOfEnemies = _history.Encounters.SelectMany(e => e).ToHashSet();
        var allUnusedElitesIds = ctx.PossibleEnemies
            .Where(e => e.Tier == EnemyTier.Elite)
            .Where(e => !historyOfEnemies.Contains(e.EnemyId))
            .Select(e => e.EnemyId)
            .ToHashSet();
        if (allUnusedElitesIds.Any())
            return ctx.WithPossibilities(e => e.Tier != EnemyTier.Elite || allUnusedElitesIds.Contains(e.EnemyId));
        
        return ctx;
    }
}
