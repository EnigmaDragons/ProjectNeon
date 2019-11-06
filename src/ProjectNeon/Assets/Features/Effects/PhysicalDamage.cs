public sealed class PhysicalDamage : Effect
{
    public float Multiplier { get; }
    public float Damage { get; private set; }

    public PhysicalDamage(float multiplier)
    {
        Multiplier = multiplier;
    }

    public void Apply(Member source, Target target)
    {
        Damage = source.State.Attack() * Multiplier;
        target.Members[0].State.TakePhysicalDamage(source.State.Attack() * Multiplier);
    }

}
