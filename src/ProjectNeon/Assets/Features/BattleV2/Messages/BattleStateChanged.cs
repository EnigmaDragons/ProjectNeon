public sealed class BattleStateChanged
{
    public BattleState State { get; }

    public BattleStateChanged(BattleState s) => State = s;
}
