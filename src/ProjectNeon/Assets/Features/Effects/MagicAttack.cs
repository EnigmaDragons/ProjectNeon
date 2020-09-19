using System.Linq;

public class MagicAttack : Effect
{
    private readonly SpellDamage _damageCalc;
    private readonly bool _hitsRandomTarget;

    public MagicAttack(float damageFactor, bool hitsRandomTarget)
    {
        _damageCalc = new SpellDamage(damageFactor);
        _hitsRandomTarget = hitsRandomTarget;
    }
    
    public void Apply(EffectContext ctx)
    {
        var applicableTargets = ctx.Target.Members.Where(x => x.IsConscious()).ToArray();
        var selectedTarget = _hitsRandomTarget && applicableTargets.Any() ? (Target)new Single(applicableTargets.Random()) : new Multiple(applicableTargets);
        
        // Processing Double Damage
        var damage = ctx.Source.State[TemporalStatType.DoubleDamage] > 0 ? _damageCalc.WithFactor(2) : _damageCalc;
        ctx.Source.State.AdjustDoubleDamage(-1);
        var effect = new DealDamage(damage);
        
        foreach (var member in selectedTarget.Members)
        {
            if (member.State[TemporalStatType.Spellshield] > 0)
                member.State.AdjustSpellshield(-1);
            else
                effect.Apply(ctx);
        }
    }
}
