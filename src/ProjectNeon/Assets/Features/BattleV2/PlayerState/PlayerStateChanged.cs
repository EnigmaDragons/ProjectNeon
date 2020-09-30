public sealed class PlayerStateChanged
{
    public PlayerState Current { get; }

    public PlayerStateChanged(PlayerState s) => Current = s;
}
