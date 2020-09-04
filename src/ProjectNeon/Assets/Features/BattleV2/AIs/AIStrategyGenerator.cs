using System.Linq;

public sealed class AIStrategyGenerator
{
    private readonly TeamType _forTeam;

    public AIStrategyGenerator(TeamType forTeam)
    {
        _forTeam = forTeam;
    }

    public AIStrategy Generate(BattleState s)
    {
        var relevantEnemies = _forTeam == TeamType.Enemies 
            ? s.Heroes.Where(h => h.IsConscious()) 
            : s.Enemies.Where(e => e.IsConscious());

        var vulnerableEnemies = relevantEnemies.Where(e => e.IsVulnerable()).ToArray();
        var preferredSingleTarget = vulnerableEnemies.Any() 
            ? vulnerableEnemies.Random() 
            : relevantEnemies.Random();
        
        return new AIStrategy(preferredSingleTarget, new Multiple(relevantEnemies.ToArray()));
    }
}
