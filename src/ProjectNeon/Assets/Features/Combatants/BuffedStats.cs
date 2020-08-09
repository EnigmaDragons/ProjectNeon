using System;

public sealed class BuffedStats : ITemporalState
{
    private int _remainingTurnDuration;
    private bool _isDebuff;
    
    public IStats Stats { get; }
    public bool IsDebuff => _isDebuff;
    public bool IsActive => _remainingTurnDuration > 0;
    public void AdvanceTurn() => _remainingTurnDuration = Math.Max(_remainingTurnDuration - 1, 0);

    public BuffedStats(IStats stats, int duration, bool isDebuff = false)
    {
        Stats = stats;
        _isDebuff = isDebuff;
        // This makes it so that for 1 Turns in the UI will last until the End of the Subsequent Turn. 
        // 0 Turn Duration would last until the end of the current turn
        // Perhaps we need a design that would select whether something should last until the start of turn, or end of turn, or perhaps the character's next action
        // For now, this solves the current use case.
        _remainingTurnDuration = duration + 1; 
    }
}
