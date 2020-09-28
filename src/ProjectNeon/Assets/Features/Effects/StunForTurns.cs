
public sealed class StunForTurns : ITemporalState
{
    private readonly int _originalDuration;
    private int _remainingDuration;

    public IStats Stats => new StatAddends().With(TemporalStatType.TurnStun, _remainingDuration);
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0;
    public IPayloadProvider OnTurnStart() => new NoPayload();
    public IPayloadProvider OnTurnEnd()
    {
        _remainingDuration--;
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() => new StunForTurns(_originalDuration);
    
    public StunForTurns(int duration)
    {
        _originalDuration = duration;
        _remainingDuration = duration;
    }
}
