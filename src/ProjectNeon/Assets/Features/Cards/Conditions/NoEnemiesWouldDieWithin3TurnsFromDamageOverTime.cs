using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesWouldDieWithin3TurnsFromDamageOverTime")]
public class NoEnemiesWouldDieWithin3TurnsFromDamageOverTime : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(enemy =>
        {
            var damage = 0;
            var dots = enemy.State.DamageOverTimes().ToArray();
            for (var i = 0; i < 3; i++)
            {
                var isVulnerable = enemy.State[TemporalStatType.Vulnerable] - 1 - i > 0;
                var dotsThisTurn = dots.Where(x => x.RemainingTurns.Value < 0 || x.RemainingTurns.Value > i).ToArray();
                damage += dotsThisTurn.Sum(x =>
                    isVulnerable ? Mathf.CeilToInt(x.Amount.Value * 1.33f) : x.Amount.Value);
            }
            return damage < enemy.CurrentHp();
        });
    
    public override string Description => "Thoughts/Condition023".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition023" };
}