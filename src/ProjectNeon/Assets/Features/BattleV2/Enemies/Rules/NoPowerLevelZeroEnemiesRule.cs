public class NoPowerLevelZeroEnemiesRule : EncounterBuildingRule
{
    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.WithPossibilities(x =>
        {
            if (x.PowerLevel != 0) 
                return true;
            Log.Error($"Enemy {x.Name} has power level 0");
            return false;
        });
}