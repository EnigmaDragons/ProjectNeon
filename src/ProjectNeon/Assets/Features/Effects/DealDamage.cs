using System.Linq;
using UnityEngine;

public sealed class DealDamage : Effect
{
    private readonly DamageCalculation _damage;

    public DealDamage(DamageCalculation damage)
    {
        _damage = damage;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Dodge, ctx.Target.Members);
        var preventingMembers = ctx.Preventions.GetPreventingMembers(PreventionType.Dodge).Select(m => m.Id);
        ctx.Target.Members.ForEach(m =>
        {
            var amount = Mathf.CeilToInt(_damage.Calculate(ctx, m) * m.State[StatType.Damagability]);
            var wasVulnerableString = m.IsVulnerable() ? " (Vulnerable) " : " ";
            if (preventingMembers.Any(id => m.Id == id))
                return;
            //BattleLog.Write($"{m.Name} prevented damage with a {TemporalStatType.Dodge}");
            else if (m.State[StatType.Damagability] < 0.01)
                BattleLog.Write($"0 damage was dealt to Invincible {m.Name}");
            else
            {
                BattleLog.Write($"{amount} damage dealt to{wasVulnerableString}{m.Name}");
                m.State.TakeDamage(amount);
            }
        });
    }
}
