using System;

public sealed class DebuffedStats : ITemporalState
{
    private int _remainingTurnDuration;
    
    public IStats Stats { get; }
    public bool IsDebuff => true;
    public bool IsActive => _remainingTurnDuration > 0;
    public void AdvanceTurn() => _remainingTurnDuration = Math.Max(_remainingTurnDuration - 1, 0);

    public DebuffedStats(IStats stats, int duration)
    {
        Stats = stats;
        _remainingTurnDuration = duration;
    }
}
