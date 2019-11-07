public sealed class Damage : Effect
{
    private DamageCalculation _damage;

    public Damage(DamageCalculation damage)
    {
        _damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        target.Members[0].State.ChangeHp(-_damage.Calculate(source, target) * target.Members[0].State.Damagability());
    }
}
