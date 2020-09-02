public sealed class DamageOverTime : Effect
{
    private readonly int _amount;
    private readonly int _turns;

    public DamageOverTime(EffectData data)
    {
        _amount = data.IntAmount;
        _turns = data.NumberOfTurns;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(new DamageOverTimeState(_amount, x, _turns)));
    }
}