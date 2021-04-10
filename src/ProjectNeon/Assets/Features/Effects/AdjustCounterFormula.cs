
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
        TemporalStatType.CardStun.ToString()
    };

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var impactSign = _negativeEffectScopes.Contains(_e.EffectScope) ? -1 : 1;
            var formulaAmount = Formula.Evaluate(ctx.SourceSnapshot.State, m.State, ctx.XPaidAmount, _e.Formula);

            var isDebuff = impactSign * formulaAmount < 0; 
            if (isDebuff)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, new [] { m });
            
            if (isDebuff && ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented {_e.EffectType} with an Aegis");
            else
                m.State.Adjust(_e.EffectScope, formulaAmount);
        });
    }
}
