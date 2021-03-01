using System.Linq;

public class MinionTeamRule : EncounterBuildingRule
{
    private readonly float _minionTeamChance;
    private readonly int _maxEnemies;

    public MinionTeamRule(float minionTeamChance, int maxEnemies)
    {
        _minionTeamChance = minionTeamChance;
        _maxEnemies = maxEnemies;
    }

    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
    {
        if (ctx.PossibleEnemies.All(x => x.Tier == EnemyTier.Minion) 
            || ctx.PossibleEnemies.All(x => x.Tier != EnemyTier.Minion) 
            || ctx.SelectedEnemies.Any(x => x.Tier != EnemyTier.Minion))
            return ctx; 
        return (ctx.SelectedEnemies.Length == 0 
                    && Rng.Chance(_minionTeamChance) 
                    && ctx.PossibleEnemies.Where(x => x.Tier == EnemyTier.Minion).Average(x => x.PowerLevel) * _maxEnemies >= ctx.TargetDifficulty)
                || ctx.SelectedEnemies.Length > 1
            ? ctx.WithPossibilities(x => x.Tier == EnemyTier.Minion)
            : ctx.WithPossibilities(x => x.Tier != EnemyTier.Minion);
    }
}