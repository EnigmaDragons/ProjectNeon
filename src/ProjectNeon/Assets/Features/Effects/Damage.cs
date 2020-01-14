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
        var amount = Mathf.CeilToInt(_damage.Calculate(source, target) * target.Members[0].State.Damagability());
        if (target.Members[0].State.Damagability() < 0.01)
            Debug.LogWarning($"{target.Members[0].Name} is Invincible");
        else if (amount < 1)
            Debug.LogWarning($"Dealing {amount} to {target.Members[0].Name}");
        else
            Debug.Log($"Dealing {amount} to {target.Members[0].Name}");
        target.Members[0].State.ChangeHp(-amount);
    }
}
