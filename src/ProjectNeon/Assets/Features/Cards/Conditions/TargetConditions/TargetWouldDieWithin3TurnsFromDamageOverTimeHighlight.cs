using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/WouldDieWithin3TurnsHighlight")]
public class TargetWouldDieWithin3TurnsFromDamageOverTimeHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(enemy =>
        {
            var damage = 0;
            var dots = enemy.State.DamageOverTimes().ToArray();
            for (var i = 0; i < 3; i++)
            {
                var isVulnerable = enemy.State[TemporalStatType.Vulnerable] - 1 - i > 0;
                var dotsThisTurn = dots.Where(x => x.RemainingTurns.Value < 0 || x.RemainingTurns.Value > i).ToArray();
                damage += dotsThisTurn.Sum(x =>
                    isVulnerable ? Mathf.CeilToInt(x.Amount.Value * 1.5f) : x.Amount.Value);
            }
            return damage >= enemy.CurrentHp();
        });
}