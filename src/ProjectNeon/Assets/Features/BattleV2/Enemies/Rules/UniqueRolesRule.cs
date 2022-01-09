using System.Collections.Generic;
using System.Linq;

public class UniqueRolesRule : EncounterBuildingRule
{
    private readonly HashSet<BattleRole> UniqueRoles = new HashSet<BattleRole>
    {
    };

    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
    {
        var takenRoles = new HashSet<BattleRole>(UniqueRoles.Where(role => ctx.SelectedEnemies.Any(enemy => enemy.Role == role)));
        return ctx.WithPossibilities(x => !takenRoles.Contains(x.Role));
    }
}