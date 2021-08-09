using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetWouldDieWithin3TurnsFromDamageOverTime")]
public class TargetWouldDieWithin3TurnsFromDamageOverTime : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var survivingTargets = ctx.Target.Members.Where(m =>
        {
            var damage = 0;
            var dots = m.State.DamageOverTimes().ToArray();
            for (var i = 0; i < 3; i++)
            {
                var isVulnerable = m.State[TemporalStatType.Vulnerable] - 1 - i > 0;
                var dotsThisTurn = dots.Where(x => x.RemainingTurns.Value < 0 || x.RemainingTurns.Value > i).ToArray();
                damage += dotsThisTurn.Sum(x => isVulnerable ? Mathf.CeilToInt(x.Amount.Value * 1.33f) : x.Amount.Value);
            }
            return damage < m.CurrentHp();
        });
        return survivingTargets.Any()
            ? new Maybe<string>($"{survivingTargets.Names()} would survive")
            : Maybe<string>.Missing();
    }
}