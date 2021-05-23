
public class ResolveInnerEffect : Effect
{
    private readonly EffectData[] _e;
    
    public ResolveInnerEffect(EffectData[] e) => _e = e;

    public void Apply(EffectContext ctx) => _e.ForEach(e => AllEffects.Apply(e, ctx));
}