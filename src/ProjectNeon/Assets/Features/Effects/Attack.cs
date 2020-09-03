using System;
using System.Linq;

[Obsolete("Needs to be replaced with the final Attack Performed data. Should be separated from Attack Proposals.")]
public sealed class Attack  : Effect
{
    private readonly bool _hitsRandomTarget;

    public Target Target { get; set; }
    private Member Attacker { get; set; }
    private Effect Effect { get; set; }
    private DamageCalculation Damage { get; set; }
    private float Multiplier { get; set; }

    public Attack(float multiplier, bool hitsRandomTarget = false)
    {
        Multiplier = multiplier;
        Damage = new PhysicalDamage(Multiplier);
        Effect = new DealDamage(Damage);
        _hitsRandomTarget = hitsRandomTarget;
    }

    public void Apply(EffectContext ctx)
    {
        Attacker = ctx.Source;
        Target = ctx.Target;
        //PROPOSALS SHOULD NOT GO THROUGH THE EVENT SYSTEM
        Message.Publish(new Proposed<Attack> { Message = this });
        var applicableTargets = Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = _hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : (Target)new Multiple(applicableTargets);
        foreach (var member in selectedTarget.Members)
        {
            if (member.State[TemporalStatType.Evade] > 0)
                member.State.AdjustEvade(-1);
            else 
                Effect.Apply(Attacker, member);
        }
        Message.Publish(new Finished<Attack> { Message = this });
    }
}
