
using System;

public class FullContextEffect : Effect
{
    private readonly Action<EffectContext, Target> _apply;

    public FullContextEffect(Action apply) => _apply = (_, __) => apply();
    public FullContextEffect(Action<EffectContext, MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, member))) { }
    public FullContextEffect(Action<MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(applyToOne)) {}
    public FullContextEffect(Action<Target> apply) : this((src, t) => apply(t)) {}
    public FullContextEffect(Action<EffectContext, Target> apply) => _apply = apply;

    public void Apply(EffectContext ctx) => _apply(ctx, ctx.Target);
}
