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
        
        return new AIStrategy(relevantEnemies.Random(), new Multiple(relevantEnemies.ToArray()));
    }
}
