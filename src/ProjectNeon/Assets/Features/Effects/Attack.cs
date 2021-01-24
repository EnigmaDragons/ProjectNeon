using System;
using System.Linq;
using UnityEngine;

[Obsolete("Needs to be replaced with the final Attack Performed data. Should be separated from Attack Proposals.")]
public sealed class Attack : Effect
{
    private readonly bool _hitsRandomTarget;

    public Target Target { get; set; }
    private Member Attacker { get; set; }
    private PhysicalDamage Damage { get; }
    private float Multiplier { get; }

    public Attack(float multiplier, bool hitsRandomTarget = false)
        : this(0, multiplier, hitsRandomTarget) {}
    public Attack(int baseAmount, float multiplier, bool hitsRandomTarget = false)
    {
        Multiplier = multiplier;
        Damage = new PhysicalDamage(baseAmount, Multiplier);
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
        var effect = new DealDamage(damage);
        Attacker.State.AdjustDoubleDamage(-1);

        var totalHpDamageDealt = 0;
        if (Attacker.State[TemporalStatType.Blind] > 0)
        {
            Attacker.State.Adjust(TemporalStatType.Blind, -1);
            BattleLog.Write($"{Attacker.Name} was blinded, so their attack missed.");
            Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        }
        else
        {
            foreach (var member in selectedTarget.Members)
            {
                var beforeHp = member.CurrentHp();
                effect.Apply(Attacker, member, ctx.Card, ctx.XPaidAmount);
                totalHpDamageDealt += beforeHp - member.CurrentHp();
            }
        }
        // Processing Lifesteal
        var lifeStealCounters = Attacker.State[TemporalStatType.Lifesteal];
        if (lifeStealCounters > 0)
        {
            var amount = lifeStealCounters * 0.25f * totalHpDamageDealt;
            Attacker.State.GainHp(amount);
            Attacker.State.Adjust(TemporalStatType.Lifesteal, -lifeStealCounters);
            BattleLog.Write($"{Attacker.Name} gained {amount} HP from LifeSteal");
        }

        Message.Publish(new Finished<Attack> { Message = this });
    }
}
