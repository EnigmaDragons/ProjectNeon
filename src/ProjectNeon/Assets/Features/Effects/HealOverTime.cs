using System;

public sealed class HealOverTime : Effect
{
    private int _turns;
    private float _multiplier;

    public HealOverTime(float multiplier, int turns) {
        _turns = turns;
        _multiplier = multiplier;
    }
    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(new HealOverTimeState((int)Math.Ceiling(_multiplier * ctx.Source.Magic()), x, _turns)));
    }
}
