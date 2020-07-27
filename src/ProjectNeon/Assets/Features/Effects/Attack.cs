
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
        //PROPOSALS SHOULD NOT GO THROUGH THE EVENT SYSTEM
        Message.Publish(new Proposed<Attack> { Message = this });
        Effect.Apply(Attacker, Target);
        Message.Publish(new Finished<Attack> { Message = this });
    }
}
