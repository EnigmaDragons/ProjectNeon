using System.Linq;

public class EncounterEnemySelector
{
    private readonly EncounterBuildingRule[] _rules;

    public EncounterEnemySelector(params EncounterBuildingRule[] rules)
        => _rules = rules;

    public bool TryGetEnemy(EncounterBuildingContext ctx, out EnemyInstance enemy)
    {
        _rules.ForEach(x => ctx = x.Filter(ctx));
        enemy = ctx.PossibleEnemies.Shuffled().FirstOrDefault();
        return ctx.PossibleEnemies.Any();
    }
}