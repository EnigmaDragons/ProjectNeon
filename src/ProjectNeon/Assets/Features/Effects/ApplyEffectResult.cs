
public class ApplyEffectResult
{
    public bool WasApplied { get; }
    public EffectContext UpdatedContext { get; }

    public ApplyEffectResult(bool wasApplied, EffectContext updatedContext)
    {
        WasApplied = wasApplied;
        UpdatedContext = updatedContext;
    }
}
