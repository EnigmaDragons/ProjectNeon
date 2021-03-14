using System.Linq;
using UnityEngine;

public class MagicAttack : Effect
{
    private readonly SpellDamage _damageCalc;
    private readonly bool _hitsRandomTarget;

    public MagicAttack(SpellDamage damageCalc, bool hitsRandomTarget)
    {
        _damageCalc = damageCalc;
        _hitsRandomTarget = hitsRandomTarget;
    }
    
    public void Apply(EffectContext ctx)
    {
        var applicableTargets = ctx.Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = _hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : new Multiple(applicableTargets);
        
        // Processing Double Damage
        var damage = ctx.Source.State[TemporalStatType.DoubleDamage] > 0 ? _damageCalc.WithFactor(2) : _damageCalc;
        ctx.Source.State.AdjustDoubleDamage(-1);

        //Process Team Shields
        var enemyEffect = new DealDamage(damage);
        var heroEffect = new DealDamage(damage);
        foreach (var teamState in ctx.TeamStates)
        {
            var damageToShield = damage.Calculate(ctx);
            var modifiedDamage = damage.WithAdjustment(damageToShield - Mathf.Min(teamState.Shields, damageToShield));
            if (teamState.Team == TeamType.Party)
                heroEffect = new DealDamage(modifiedDamage);
            else 
                enemyEffect = new DealDamage(modifiedDamage);
            ctx.TeamStates[0].Adjust(TeamStatType.Shield, damageToShield);
        }

        var totalHpDamageDealt = 0;
        foreach (var member in selectedTarget.Members)
        {
            var beforeHp = member.CurrentHp();
            var effect = member.TeamType == TeamType.Party ? heroEffect : enemyEffect;
            effect.Apply(ctx.Retargeted(ctx.Source, new Single(member)));
            totalHpDamageDealt += beforeHp - member.CurrentHp();
        }

        // Processing Lifesteal
        var lifeStealCounters = ctx.Source.State[TemporalStatType.Lifesteal];
        if (lifeStealCounters > 0)
        {
            var amount = lifeStealCounters * 0.25f * totalHpDamageDealt;
            ctx.Source.State.GainHp(amount);
            ctx.Source.State.Adjust(TemporalStatType.Lifesteal, -lifeStealCounters);
            BattleLog.Write($"{ctx.Source.Name} gained {amount} HP from LifeSteal");
        }
    }
}
