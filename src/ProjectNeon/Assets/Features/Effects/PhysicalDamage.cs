public sealed class PhysicalDamage : Effect
{
    public int Damage { get; }

    public PhysicalDamage(int damage)
    {
        Damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        target.Members[0].State.TakePhysicalDamage(source.State.Attack() * Damage);
    }

}
