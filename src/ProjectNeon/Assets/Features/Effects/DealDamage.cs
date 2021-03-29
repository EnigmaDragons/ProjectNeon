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
        ctx.Target.Members.ForEach(m =>
        {
            var amount = Mathf.CeilToInt(_damage.Calculate(ctx, m) * m.State.Damagability());
            var wasVulnerableString = m.IsVulnerable() ? " (Vulnerable) " : " ";
            if (m.State.Damagability() < 0.01)
                BattleLog.Write($"0 damage was dealt to Invincible {m.Name}");
            else
                BattleLog.Write($"{amount} damage dealt to{wasVulnerableString}{m.Name}");
            m.State.TakeDamage(amount);
        });
    }
}
