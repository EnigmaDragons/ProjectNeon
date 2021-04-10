
using System.Collections.Generic;
using System.Linq;

public class AdjustCounterFormula : Effect
{
    private readonly EffectData _e;

    public AdjustCounterFormula(EffectData e) => _e = e;

    private readonly HashSet<string> _aegisEffectScopes = new HashSet<string>
    {
        TemporalStatType.Disabled.ToString(),
        TemporalStatType.Blind.ToString(),
        TemporalStatType.Confused.ToString(),
        TemporalStatType.Inhibit.ToString(),
        TemporalStatType.CardStun.ToString()
    };
    
    public void Apply(EffectContext ctx)
    {
        if (_aegisEffectScopes.Contains(_e.EffectScope.Value))
            ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, ctx.Target.Members.GetConscious());
        var aegisMembers = ctx.Preventions.GetPreventingMembers(PreventionType.Aegis).Select(m => m.Id);
        ctx.Target.ApplyToAllConscious(m => 
        {
            if (aegisMembers.Contains(m.MemberId))
                BattleLog.Write($"{m.Name} prevented {_e.EffectType} with an Aegis");
            else
                m.Adjust(_e.EffectScope, Formula.Evaluate(ctx.SourceSnapshot.State, m, ctx.XPaidAmount, _e.Formula));
        });
    }
}
