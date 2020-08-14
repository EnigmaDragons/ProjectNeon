public sealed class BattleStateChanged
{
    public BattleStateSnapshot Before { get; }
    public BattleState State { get; }

    public BattleStateChanged(BattleStateSnapshot before, BattleState after)
    {
        Before = before;
        State = after;
    }
}
