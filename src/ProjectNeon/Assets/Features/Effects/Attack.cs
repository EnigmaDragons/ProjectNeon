using System;
using System.Linq;
using UnityEngine;

[Obsolete("Needs to be replaced with the final Attack Performed data. Should be separated from Attack Proposals.")]
public sealed class Attack  : Effect
{
    private readonly bool _hitsRandomTarget;

    public Target Target { get; set; }
    private Member Attacker { get; set; }
    private PhysicalDamage Damage { get; }
    private float Multiplier { get; }

    public Attack(float multiplier, bool hitsRandomTarget = false)
    {
        Multiplier = multiplier;
        Damage = new PhysicalDamage(Multiplier);
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

        // Processing Double Damage
        var damage = Attacker.State[TemporalStatType.DoubleDamage] > 0 ? Damage.WithFactor(2) : Damage;
        Attacker.State.AdjustDoubleDamage(-1);

        if (Attacker.State[TemporalStatType.Blind] > 0)
        {
            Attacker.State.Adjust(TemporalStatType.Blind, -1);
            BattleLog.Write($"{Attacker.Name} was blinded, so their attack missed.");
            Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        }
        else
        {
            var effect = new DealDamage(damage);

            foreach (var member in selectedTarget.Members)
            {
                effect.Apply(Attacker, member);
            }
        }

        Message.Publish(new Finished<Attack> { Message = this });
    }
}
