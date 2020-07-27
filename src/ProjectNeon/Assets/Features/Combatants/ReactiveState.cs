using System;

public class ReactiveState : ITemporalState
{
    private int _remainingTurnDuration;

    public ReactiveState(Action<object> subscribe, int duration, bool isDebuff)
    {
        _remainingTurnDuration = duration;
        Stats = new StatAddends();
        IsDebuff = isDebuff;
        if (IsActive)
        {
            subscribe(this);
            Message.Subscribe<BattleFinished>(_ => Message.Unsubscribe(this), this);
        }
    }
    
    public IStats Stats { get; }
    public bool IsDebuff { get; }
    public bool IsActive => _remainingTurnDuration > 0;
    public void AdvanceTurn()
    {
        if (!IsActive)
            return;
        _remainingTurnDuration--;
        if (!IsActive)
            Message.Unsubscribe(this);
    }
}