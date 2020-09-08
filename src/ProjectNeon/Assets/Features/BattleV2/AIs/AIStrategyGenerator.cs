using System;
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
        
        var team = _forTeam == TeamType.Enemies
                   ? s.Enemies.Where(h => h.IsConscious()) 
                   : s.Heroes.Where(e => e.IsConscious());
        var designatedAttacker = team
            .OrderByDescending(e => e.BattleRole == BattleRole.Striker ? 1 : 0)
            .ThenByDescending(e => Math.Max(e.State.Attack(), e.State.Magic()))
            .First();
        
        return new AIStrategy(preferredSingleTarget, new Multiple(relevantEnemies.ToArray()), designatedAttacker);
    }
}
