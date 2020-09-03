using System.Linq;

public class MagicAttack : Effect
{
    private readonly SpellDamage _damageCalculation;
    private readonly bool _hitsRandomTarget;

    public MagicAttack(float damageFactor, bool hitsRandomTarget)
    {
        _damageCalculation = new SpellDamage(damageFactor);
        _hitsRandomTarget = hitsRandomTarget;
    }
    
    public void Apply(EffectContext ctx)
    {
        var applicableTargets = ctx.Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = _hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : new Multiple(applicableTargets);
        foreach (var member in selectedTarget.Members)
        {
            if (member.State[TemporalStatType.Spellshield] > 0)
                member.State.AdjustSpellshield(-1);
            else
                member.State.TakeDamage(_damageCalculation.Calculate(ctx.Source, member));
        }
    }
}
