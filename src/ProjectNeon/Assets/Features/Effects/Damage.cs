using UnityEngine;

public sealed class Damage : Effect
{
    private readonly DamageCalculation _damage;

    public Damage(DamageCalculation damage)
    {
        _damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        target.Members.ForEach(m =>
        {
            var amount = Mathf.CeilToInt(_damage.Calculate(source, m) * m.State.Damagability());
            if (target.Members[0].State.Damagability() < 0.01)
                BattleLog.Write($"{m.Name} is Invincible");
            else if (amount < 1)
                BattleLog.Write($"Dealing {amount} to {m.Name}");
            else
                BattleLog.Write($"Dealing {amount} to {m.Name}");
            m.State.TakeDamage(amount);
        });
    }
}
