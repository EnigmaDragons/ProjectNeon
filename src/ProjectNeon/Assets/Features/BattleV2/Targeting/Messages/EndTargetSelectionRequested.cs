public class EndTargetSelectionRequested
{
    public bool ShouldDiscard { get; }

    public EndTargetSelectionRequested(bool shouldDiscard) => ShouldDiscard = shouldDiscard;
}