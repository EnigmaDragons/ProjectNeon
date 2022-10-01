public class StartNewGameRequested
{
    public AdventureMode Mode { get; }

    public StartNewGameRequested(AdventureMode mode) => Mode = mode;
}
