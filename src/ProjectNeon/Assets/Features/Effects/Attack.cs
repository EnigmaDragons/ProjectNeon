using System;
using System.Linq;
using UnityEngine;

[Obsolete("Needs to be replaced with the final Attack Performed data. Should be separated from Attack Proposals.")]
public sealed class Attack : Effect
{
    private readonly bool _hitsRandomTarget;

    public Target Target { get; set; }
    private PhysicalDamage Damage { get; }
    
    public Attack(PhysicalDamage damage, bool hitsRandomTarget = false)
    {
        Damage = damage;
        _hitsRandomTarget = hitsRandomTarget;
    }

    public void Apply(EffectContext ctx)
    {
        var attacker = ctx.Source;
        Target = ctx.Target;
        //PROPOSALS SHOULD NOT GO THROUGH THE EVENT SYSTEM
        Message.Publish(new Proposed<Attack> { Message = this });
        var applicableTargets = Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = _hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : (Target)new Multiple(applicableTargets);

        // Processing Double Damage
        var damage = attacker.State[TemporalStatType.DoubleDamage] > 0 ? Damage.WithFactor(2) : Damage;
        var effect = new DealDamage(damage);
        attacker.State.AdjustDoubleDamage(-1);
        
        var totalHpDamageDealt = 0;
        if (attacker.State[TemporalStatType.Blind] > 0)
        {
            attacker.State.Adjust(TemporalStatType.Blind, -1);
            BattleLog.Write($"{attacker.Name} was blinded, so their attack missed.");
            Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        }
        else
        {
            foreach (var member in selectedTarget.Members)
            {
                var beforeHp = member.CurrentHp();
                effect.Apply(ctx.Retargeted(attacker, new Single(member)));
                totalHpDamageDealt += beforeHp - member.CurrentHp();
            }
        }
        // Processing Lifesteal
        var lifeStealCounters = attacker.State[TemporalStatType.Lifesteal];
        if (lifeStealCounters > 0)
        {
            var amount = lifeStealCounters * 0.25f * totalHpDamageDealt;
            attacker.State.GainHp(amount);
            attacker.State.Adjust(TemporalStatType.Lifesteal, -lifeStealCounters);
            BattleLog.Write($"{attacker.Name} gained {amount} HP from LifeSteal");
        }

        Message.Publish(new Finished<Attack> { Message = this });
    }
}
