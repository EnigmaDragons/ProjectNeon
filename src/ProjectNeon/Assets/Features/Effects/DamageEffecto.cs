public sealed class DamageEffecto : Effect
{
    private Damage _damage;

    public DamageEffecto(Damage damage)
    {
        _damage = damage;
    }

    void Effect.Apply(Member source, Target target)
    {
        target.Members[0].State.ChangeHp(-_damage.Calculate(source, target) * target.Members[0].State.Damagability());
    }
}
