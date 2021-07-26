using System.Linq;

public static class AttackProcessing
{
    public static void Apply(DamageCalculation dmg, bool hitsRandomTarget, EffectContext ctx)
    {
        var attacker = ctx.Source;
        var applicableTargets = ctx.Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : new Multiple(applicableTargets);

        // Processing Double Damage
        var usesDoubleDamage = attacker.State[TemporalStatType.DoubleDamage] > 0;
        var damage = usesDoubleDamage ? dmg.WithFactor(2) : dmg;
        if (usesDoubleDamage)
            attacker.State.Adjust(TemporalStatType.DoubleDamage, -1);

        var effect = new DealDamage(damage);
        var totalHpDamageDealt = 0;
        foreach (var member in selectedTarget.Members)
        {
            var beforeHp = member.CurrentHp();
            effect.Apply(ctx.Retargeted(attacker, new Single(member)));
            var amountDealt = beforeHp - member.CurrentHp();
            totalHpDamageDealt += amountDealt;
            if (usesDoubleDamage && amountDealt > 0)
                Message.Publish(new DisplayCharacterWordRequested(member, "Double Damage!"));
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
    }
}