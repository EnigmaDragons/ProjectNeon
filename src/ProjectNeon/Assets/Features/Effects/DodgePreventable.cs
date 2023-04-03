using System.Linq;

public class DodgePreventable : Effect
{
    private readonly Effect _inner;
    private readonly string _effectDescription;

    public DodgePreventable(Effect inner, string effectDescription)
    {
        _inner = inner;
        _effectDescription = effectDescription;
    }
    
    public void Apply(EffectContext ctx)
    {
        var consciousTargets = ctx.Target.Members.GetConscious();
        ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Dodge, consciousTargets);
        var dodgeMemberIds = ctx.Preventions.GetPreventingMembers(PreventionType.Dodge).Select(m => m.Id);
        consciousTargets.ForEach(m =>
        {
            if (dodgeMemberIds.Contains(m.Id))
                BattleLog.Write($"{m.UnambiguousEnglishName} prevented {_effectDescription} with a Dodge");
            else
                _inner.Apply(ctx.Retargeted(ctx.Source, new Single(m)));
        });
    }
    
    public static Effect If(Effect e, bool condition, string effectDescription) 
        => condition 
            ? new DodgePreventable(e, effectDescription) 
            : e;
}