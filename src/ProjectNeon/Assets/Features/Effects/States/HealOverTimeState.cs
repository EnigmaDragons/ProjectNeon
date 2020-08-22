public class HealOverTimeState : ITemporalState
{
    private readonly int _amount;
    private readonly Member _target;
    private readonly bool _indefinite;
    private int _remainingDuration;

    public IStats Stats { get; }
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0 || _indefinite;

    public HealOverTimeState(int amount, Member target, int turns)
    {
        Stats = new StatAddends();
        _amount = amount;
        _target = target;
        _indefinite = turns < 0;
        _remainingDuration = turns;
    }

    public void OnTurnStart()
    {
        if (!IsActive) return;

        _remainingDuration--;
        _target.State.GainHp(_amount);
    }

    public void OnTurnEnd() {}
}