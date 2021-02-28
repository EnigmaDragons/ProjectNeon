using System.Linq;

public class OneEliteRule : EncounterBuildingRule
{
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.PossibleEnemies.Any(x => x.Tier == EnemyTier.Elite)
            ? ctx.SelectedEnemies.Any(x => x.Tier == EnemyTier.Elite)
                ? ctx.WithPossibilities(x => x.Tier != EnemyTier.Elite)
                : ctx.WithPossibilities(new Enemy[] { ctx.PossibleEnemies.Shuffled().First(x => x.Tier == EnemyTier.Elite) })
            : ctx;
}