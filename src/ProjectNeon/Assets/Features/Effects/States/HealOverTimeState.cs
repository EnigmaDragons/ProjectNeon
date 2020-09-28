public class HealOverTimeState : ITemporalState
{
    private readonly TemporalStateTracker _tracker;
    private readonly int _amount;
    private readonly Member _target;

    public IStats Stats { get; }
    public StatusTag Tag => StatusTag.HealOverTime;
    public bool IsDebuff => false;
    public bool IsActive => _tracker.IsActive;

    public HealOverTimeState(int amount, Member target, int turns)
        : this(amount, target, TemporalStateMetadata.BuffForDuration(turns)) {}
    
    public HealOverTimeState(int amount, Member target, TemporalStateMetadata metadata)
    {
        Stats = new StatAddends();
        _amount = amount;
        _target = target;
        _tracker = new TemporalStateTracker(metadata);
    }

    public void OnTurnStart()
    {
        if (!IsActive || !_target.IsConscious()) return;

        _tracker.AdvanceTurn();
        _target.State.GainHp(_amount);
    }

    public void OnTurnEnd() {}
    
    public ITemporalState CloneOriginal() => new HealOverTimeState(_amount, _target, _tracker.Metadata);
}