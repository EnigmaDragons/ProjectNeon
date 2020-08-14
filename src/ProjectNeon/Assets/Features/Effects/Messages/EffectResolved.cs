public sealed class EffectResolved
{
    public EffectData EffectData { get; }
    public Member Source { get; }
    public Target Target { get; }
    public BattleStateSnapshot BattleBefore { get; }
    public BattleStateSnapshot BattleAfter { get; }

    public EffectResolved(EffectData e, Member src, Target target, BattleStateSnapshot before, BattleStateSnapshot after)
    {
        EffectData = e;
        Source = src;
        Target = target;
        BattleBefore = before;
        BattleAfter = after;
    }
}
