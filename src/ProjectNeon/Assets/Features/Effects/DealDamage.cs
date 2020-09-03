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
            var amount = Mathf.CeilToInt(_damage.Calculate(ctx.Source, m) * m.State.Damagability());
            if (m.State.Damagability() < 0.01)
                BattleLog.Write($"{m.Name} is Invincible");
            else
                BattleLog.Write($"Dealing {amount} to {m.Name}");
            m.State.TakeDamage(amount);
        });
    }
}
