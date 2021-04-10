using System.Linq;

public class AegisPreventable : Effect
{
    private readonly Effect _inner;
    private readonly string _effectDescription;

    public AegisPreventable(Effect inner, string effectDescription)
    {
        _inner = inner;
        _effectDescription = effectDescription;
    }
    
    public void Apply(EffectContext ctx)
    {
        var consciousTargets = ctx.Target.Members.GetConscious();
        ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, consciousTargets);
        var aegisMemberIds = ctx.Preventions.GetPreventingMembers(PreventionType.Aegis).Select(m => m.Id);
        consciousTargets.ForEach(m =>
        {
            if (aegisMemberIds.Contains(m.Id))
                BattleLog.Write($"{m.Name} prevented {_effectDescription} with an Aegis");
            else
                _inner.Apply(ctx.Retargeted(ctx.Source, new Single(m)));
        });
    }
    
    public static Effect If(Effect e, bool condition, string effectDescription) 
        => condition 
            ? new AegisPreventable(e, effectDescription) 
            : e;
}
