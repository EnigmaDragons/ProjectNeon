using System.Linq;

public class UniqueRule : EncounterBuildingRule
{
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.WithPossibilities(possible => !possible.IsUnique || ctx.SelectedEnemies.All(selected => selected.Name != possible.Name));
}