using System;
using System.Linq;

public sealed class AIStrategyGenerator
{
    private readonly TeamType _forTeam;

    public AIStrategyGenerator(TeamType forTeam)
    {
        _forTeam = forTeam;
    }

    public AIStrategy Generate(BattleState s, CardTypeData disabledCard)
    {
        var relevantEnemies = _forTeam == TeamType.Enemies 
            ? s.Heroes.Where(h => h.IsConscious()).ToArray() 
            : s.EnemyMembers.Where(e => e.IsConscious()).ToArray();
        
        var targetableEnemies = relevantEnemies.Where(x => !x.IsStealthed()).ToArray();
        var vulnerableEnemies = targetableEnemies.Where(e => e.IsVulnerable()).ToArray();
        var tauntEnemies = targetableEnemies.Where(e => e.HasTaunt()).ToArray();
        var preferredSingleTarget =
            tauntEnemies.Any() 
                ? tauntEnemies.Random()
                : vulnerableEnemies.Any() 
                    ? vulnerableEnemies.Random() 
                    : targetableEnemies.Any()
                        ? targetableEnemies.Random()
                        : Maybe<Member>.Missing();
        
        var team = _forTeam == TeamType.Enemies
                   ? s.EnemyMembers.Where(h => h.IsConscious()) 
                   : s.Heroes.Where(e => e.IsConscious());
        var designatedAttacker = team
            .OrderByDescending(e => 
                e.BattleRole == BattleRole.Striker ? 2 : 
                e.BattleRole == BattleRole.Bruiser ? 1 : 
                0)
            .ThenByDescending(e => Math.Max(e.State.Attack(), e.State.Magic()))
            .First();
        
        return new AIStrategy(preferredSingleTarget, new Multiple(relevantEnemies.ToArray()), designatedAttacker, disabledCard);
    }

    public AIStrategy Update(AIStrategy strategy, BattleState s)
    {
        if (strategy.SingleMemberAttackTarget.IsPresent && strategy.SingleMemberAttackTarget.Value.IsConscious() && !strategy.SingleMemberAttackTarget.Value.IsStealthed()) 
            return strategy;
        var relevantEnemies = _forTeam == TeamType.Enemies 
            ? s.Heroes.Where(h => h.IsConscious()).ToArray() 
            : s.EnemyMembers.Where(e => e.IsConscious()).ToArray();
        var targetableEnemies = relevantEnemies.Where(x => !x.IsStealthed()).ToArray();
        var tauntEnemies = targetableEnemies.Where(e => e.HasTaunt()).ToArray();
        var preferredSingleTarget =
            tauntEnemies.Any() 
                ? tauntEnemies.Random()
                : targetableEnemies.Any()
                    ? targetableEnemies.Random()
                    : Maybe<Member>.Missing();
        return new AIStrategy(preferredSingleTarget, strategy.GroupAttackTarget, strategy.DesignatedAttacker, strategy.DisabledCard);
    }
}
