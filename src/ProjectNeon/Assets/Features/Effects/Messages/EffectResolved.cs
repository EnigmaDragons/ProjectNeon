public sealed class EffectResolved
{
    public EffectData EffectData { get; }
    public Member Source { get; }
    public Target Target { get; }
    public BattleStateChanged StateChanges { get; }

    public EffectResolved(EffectData e, Member src, Target target, BattleStateChanged changes)
    {
        EffectData = e;
        Source = src;
        Target = target;
        StateChanges = changes;
    }
}
