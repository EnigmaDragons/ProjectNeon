
using System.Linq;

public class ExcludeSelfFromEffect : Effect
{
    private Effect _origin;

    public ExcludeSelfFromEffect(Effect origin) 
    {
        _origin = origin;
    }

    public void Apply(EffectContext ctx)
    {
        var newContext = new EffectContext(
            ctx.Source,
            new Multiple(ctx.Target.Members.Except(ctx.Source).ToArray()), 
            ctx.PlayerState, 
            ctx.BattleMembers);
        
        _origin.Apply(newContext);
    }
}