using UnityEngine;

public sealed class Damage : Effect
{
    private readonly DamageCalculation _damage;

    public Damage(DamageCalculation damage)
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
            else if (amount < 1)
                BattleLog.Write($"Dealing {amount} to {m.Name}");
            else
                BattleLog.Write($"Dealing {amount} to {m.Name}");
            m.State.TakeDamage(amount);
        });
    }
}
