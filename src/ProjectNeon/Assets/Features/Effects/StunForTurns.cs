using System;

public sealed class StunForTurns : ITemporalState
{
    private int _remainingDuration;

    public IStats Stats => new StatAddends().With(TemporalStatType.TurnStun, _remainingDuration);
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0;
    public void OnTurnStart() {}
    public void OnTurnEnd() => _remainingDuration--;

    public StunForTurns(float duration) => _remainingDuration = CeilingInt(duration);
    
    private static int CeilingInt(float v) => Convert.ToInt32(Math.Ceiling(v));
}
