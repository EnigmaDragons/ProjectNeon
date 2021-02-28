using System.Linq;

public class AddStrikerOrBruiserIfThereIsNoneRule : EncounterBuildingRule
{
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.SelectedEnemies.All(x => x.Role != BattleRole.Striker && x.Role != BattleRole.Bruiser) && ctx.PossibleEnemies.Any(x => x.Role == BattleRole.Striker || x.Role == BattleRole.Bruiser)
            ? ctx.WithPossibilities(ctx.PossibleEnemies.Where(x => x.Role == BattleRole.Striker || x.Role == BattleRole.Bruiser).ToArray())
            : ctx;
}