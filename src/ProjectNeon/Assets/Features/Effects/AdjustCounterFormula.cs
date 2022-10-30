using System.Collections.Generic;
using System.Linq;

public class AdjustCounterFormula : Effect
{
    private readonly EffectData _e;

    public AdjustCounterFormula(EffectData e) => _e = e;

    private readonly HashSet<string> _negativeEffectScopes = new HashSet<string>
    {
        TemporalStatType.Disabled.ToString(),
        TemporalStatType.Blind.ToString(),
        TemporalStatType.Confused.ToString(),
        TemporalStatType.Inhibit.ToString(),
        TemporalStatType.Stun.ToString(),
        TemporalStatType.Vulnerable.ToString(),
        TemporalStatType.AntiHeal.ToString(),
        TemporalStatType.PreventResourceGains.ToString()
    };

    private readonly HashSet<string> _neutralEffectScopes = new HashSet<string>()
    {
        TemporalStatType.Prominent.ToString(),
    };

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var impactSign = _negativeEffectScopes.Contains(_e.EffectScope) ? -1 : 1;
            var formulaAmount = Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, _e.Formula, ctx.XPaidAmount);

            var isDebuff = !_neutralEffectScopes.Contains(_e.EffectScope) && (impactSign * formulaAmount < 0); 
            if (isDebuff && !_e.Unpreventable)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, new [] { m });
            
            if (isDebuff && ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.UnambiguousEnglishName} prevented {_e.EffectType} with an Aegis");
            else
                m.State.Adjust(_e.EffectScope, formulaAmount);
        });
    }
}
