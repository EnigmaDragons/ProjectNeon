public sealed class DamageApplied : Effect
{
    private Damage _damage;

    public DamageApplied(Damage damage)
    {
        _damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        target.Members[0].State.ChangeHp(-_damage.Calculate(source, target) * target.Members[0].State.Damagability());
    }
}
