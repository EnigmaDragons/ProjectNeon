
public class ResolveFirstInnerEffect : Effect
{
    private readonly EffectData _e;
    
    public ResolveFirstInnerEffect(EffectData e) => _e = e;

    public void Apply(EffectContext ctx) => AllEffects.Apply(_e, ctx);
}