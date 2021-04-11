public class StopWhenMaxedOutEnemiesRule : EncounterBuildingRule
{
    private readonly int _maxEnemies;

    public StopWhenMaxedOutEnemiesRule(int maxEnemies)
        => _maxEnemies = maxEnemies;

    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.SelectedEnemies.Length == _maxEnemies
            ? ctx.WithPossibilities(new EnemyInstance[0])
            : ctx;
}