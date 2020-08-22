using System;

public class AdjustedStats : ITemporalState
{
    private int _remainingTurnDuration;
    private bool _indefinite;

    public IStats Stats { get; }
    public bool IsDebuff { get; }
    public bool IsActive => _remainingTurnDuration == 0 && !_indefinite;
    public void OnTurnStart() { }
    public void OnTurnEnd() => _remainingTurnDuration = Math.Max(_remainingTurnDuration - 1, 0);

    public static AdjustedStats CreateIndefinite(IStats stats, bool isDebuff)
        => new AdjustedStats(stats, 0, isDebuff, true);
    public AdjustedStats(IStats stats, int duration, bool isDebuff, bool indefinite)
    {
        Stats = stats;
        IsDebuff = isDebuff;
        // This makes it so that for 1 Turns in the UI will last until the End of the Subsequent Turn. 
        // 0 Turn Duration would last until the end of the current turn
        // Perhaps we need a design that would select whether something should last until the start of turn, or end of turn, or perhaps the character's next action
        // For now, this solves the current use case.
        _remainingTurnDuration = duration + 1;
        _indefinite = indefinite;
    }
}