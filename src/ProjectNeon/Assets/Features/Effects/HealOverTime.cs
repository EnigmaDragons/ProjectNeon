using System;

public sealed class HealOverTime : Effect
{
    private int _turns;
    private float _multiplier;

    public HealOverTime(float multiplier, int turns) {
        _turns = turns;
        _multiplier = multiplier;
    }

    public void Apply(Member source, Target target)
    {
        target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(new HealOverTimeState((int)Math.Ceiling(_multiplier * source.Magic()), x, _turns)));
    }
}
