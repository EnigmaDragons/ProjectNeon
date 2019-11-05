public sealed class PhysicalDamage : Effect
{
    public float Multiplier { get; }

    public PhysicalDamage(float multiplier)
    {
        Multiplier = multiplier;
    }

    public void Apply(Member source, Target target)
    {
        target.Members[0].State.TakePhysicalDamage(source.State.Attack() * Multiplier);
    }

}
