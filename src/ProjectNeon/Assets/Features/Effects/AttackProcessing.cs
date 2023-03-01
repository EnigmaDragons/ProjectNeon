using System.Linq;

public static class AttackProcessing
{
    public static void Apply(DamageCalculation dmg, bool hitsRandomTarget, EffectContext ctx)
    {
        var attacker = ctx.Source;
        var applicableTargets = ctx.Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = hitsRandomTarget && applicableTargets.AnyNonAlloc() ? (Target)new Single(applicableTargets.Random()) : new Multiple(applicableTargets);

        var effect = new DealDamage(dmg);
        var totalHpDamageDealt = 0;
        foreach (var member in selectedTarget.Members)
        {
            var beforeHp = member.CurrentHp();
            effect.Apply(ctx.Retargeted(attacker, new Single(member)));
            var amountDealt = beforeHp - member.CurrentHp();
            totalHpDamageDealt += amountDealt;
        }
        
        // Processing Lifesteal
        var lifeStealCounters = attacker.State[TemporalStatType.Lifesteal];
        if (lifeStealCounters > 0)
        {
            var amount = lifeStealCounters * 0.25f * totalHpDamageDealt;
            attacker.State.GainHp(amount);
            attacker.State.Adjust(TemporalStatType.Lifesteal, -lifeStealCounters);
            BattleLog.Write($"{attacker.NameTerm.ToEnglish()} gained {amount} HP from LifeSteal");
        }
    }
}