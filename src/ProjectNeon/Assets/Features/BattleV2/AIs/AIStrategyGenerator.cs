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

    public AIStrategy Generate(BattleState s, CardTypeData disabledCard)
    {
        var consciousOpponents = GetConsciousOpponents(s);
        var preferredSingleTarget = GetPreferredSingleTarget(consciousOpponents);
        var designatedAttacker = SelectDesignatedAttacker(s);
        
        return new AIStrategy(preferredSingleTarget, new Multiple(consciousOpponents.ToArray()), designatedAttacker, disabledCard);
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

    private Member SelectDesignatedAttacker(BattleState s)
    {
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
        
        return designatedAttacker;
    }

    public AIStrategy Update(AIStrategy strategy, BattleState s)
    {
        return WithRefinedDesignatedAttacker(s, 
            WithRefinedPreferredSingleTarget(s, strategy));
    }

    private AIStrategy WithRefinedDesignatedAttacker(BattleState s, AIStrategy strategy)
    {
        if (!strategy.DesignatedAttacker.IsUnconscious())
            return strategy;
        return new AIStrategy(strategy.SingleMemberAttackTarget, strategy.GroupAttackTarget, SelectDesignatedAttacker(s), strategy.DisabledCard);
    }

    private AIStrategy WithRefinedPreferredSingleTarget(BattleState s, AIStrategy strategy)
    {
        if (strategy.SingleMemberAttackTarget.IsPresent && strategy.SingleMemberAttackTarget.Value.IsConscious() && !strategy.SingleMemberAttackTarget.Value.IsStealthed()) 
            return strategy;
        
        var consciousOpponents = GetConsciousOpponents(s);
        var preferredSingleTarget = GetPreferredSingleTarget(consciousOpponents);
        return new AIStrategy(preferredSingleTarget, strategy.GroupAttackTarget, strategy.DesignatedAttacker, strategy.DisabledCard);
    }
}
