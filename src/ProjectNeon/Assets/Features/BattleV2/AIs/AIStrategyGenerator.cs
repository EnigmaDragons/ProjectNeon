using System;
using System.Collections.Generic;
using System.Linq;

public sealed class AIStrategyGenerator
{
    private readonly TeamType _forTeam;

    public AIStrategyGenerator(TeamType forTeam)
    {
        _forTeam = forTeam;
    }

    public AIStrategy Generate(BattleState s, EnemySpecialCircumstanceCards specialCards)
    {
        var consciousOpponents = GetConsciousOpponents(s);
        var preferredSingleTarget = GetPreferredSingleTarget(consciousOpponents);
        var designatedAttacker = SelectDesignatedAttacker(s);
        
        return new AIStrategy(preferredSingleTarget, new Multiple(consciousOpponents.ToArray()), designatedAttacker, specialCards, new DeterministicRng(s.BattleRngSeed));
    }

    private static Maybe<Member> GetPreferredSingleTarget(Member[] relevantEnemies)
    {
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
        return preferredSingleTarget;
    }

    private Member[] GetConsciousOpponents(BattleState s)
    {
        var relevantEnemies = _forTeam == TeamType.Enemies
            ? s.Heroes.Where(h => h.IsConscious()).ToArray()
            : s.EnemyMembers.Where(e => e.IsConscious()).ToArray();
        return relevantEnemies;
    }

    private IEnumerable<Member> TeamMembers(BattleState s)
        => _forTeam == TeamType.Enemies
                ? s.EnemyMembers.Where(h => h.IsConscious()) 
                : s.Heroes.Where(e => e.IsConscious());

    private Member SelectDesignatedAttacker(BattleState s)
    {
        var team = TeamMembers(s);
        
        var designatedAttacker = team
            .OrderByDescending(e =>
                e.BattleRole == BattleRole.DamageDealer ? 2 :
                e.BattleRole == BattleRole.Specialist ? 1 :
                0)
            .ThenByDescending(e => (int)s.GetAiPreferences(e.Id).DesignatedAttackerPriority)
            .ThenByDescending(e => Math.Max(e.State.Attack(), e.State.Magic()))
            .First();
        
        return designatedAttacker;
    }

    public AIStrategy Update(AIStrategy strategy, BattleState s)
    {
        return WithRefinedDesignatedAttacker(s, 
            WithRefinedPreferredSingleTarget(s, strategy));
    }

    private AIStrategy WithRefinedDesignatedAttacker(BattleState s, AIStrategy strategy)
    {
        if (strategy.DesignatedAttacker.IsConscious() || TeamMembers(s).None())
            return strategy;
        return new AIStrategy(strategy.SingleMemberAttackTarget, strategy.GroupAttackTarget, SelectDesignatedAttacker(s), strategy.SpecialCards, strategy.Rng);
    }

    private AIStrategy WithRefinedPreferredSingleTarget(BattleState s, AIStrategy strategy)
    {
        if (strategy.SingleMemberAttackTarget.IsPresent && strategy.SingleMemberAttackTarget.Value.IsConscious() && !strategy.SingleMemberAttackTarget.Value.IsStealthed()) 
            return strategy;
        
        var consciousOpponents = GetConsciousOpponents(s);
        var preferredSingleTarget = GetPreferredSingleTarget(consciousOpponents);
        return new AIStrategy(preferredSingleTarget, strategy.GroupAttackTarget, strategy.DesignatedAttacker, strategy.SpecialCards, strategy.Rng);
    }
}
