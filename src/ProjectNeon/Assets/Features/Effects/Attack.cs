
public sealed class Attack  : Effect
{
    public Member Attacker { get; set; }
    public Target Target { get; set; }
    public Effect Effect { get; set; }
    public DamageCalculation Damage { get; set; }
    public float Multiplier { get; set; }

    public Attack(float multiplier)
    {
        Multiplier = multiplier;
        Damage = new PhysicalDamage(Multiplier);
        Effect = new Damage(Damage);
    }

    public void Apply(Member source, Target target)
    {
        Attacker = source;
        Target = target;
        Message.Publish(
            new AttackToPerform(this)
        );
        Effect.Apply(source, target);
        Message.Publish(
            new AttackPerformed(this)
        );
    }

    public void Apply(Member source, Member target) {
        Apply(source, new Single(target));
    }
}
