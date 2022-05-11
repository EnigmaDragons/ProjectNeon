
public class ResolveInnerEffect : Effect
{
    private readonly EffectData[] _e;
    
    public ResolveInnerEffect(EffectData[] e) => _e = e;

    public void Apply(EffectContext ctx) => _e.ForEach(e => Message.Publish(new ApplyBattleEffect(false, e, ctx.Source, ctx.Target, ctx.Card, ctx.XPaidAmount, ctx.Preventions, ctx.IsReaction, ctx.Timing)));
}