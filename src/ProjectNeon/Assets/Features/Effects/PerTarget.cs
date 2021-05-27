using System.Linq;

public class PerTarget : Effect
{
    private readonly EffectData _inner;

    public PerTarget(EffectData inner) => _inner = inner;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members
            .Select(m => ctx.Retargeted(ctx.Source, new Single(m)))
            .ForEach(c => AllEffects.Apply(_inner, c));
    }
}
