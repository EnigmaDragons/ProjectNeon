public class DamageOverTimeState : ITemporalState
{
    private readonly int _amount;
    private readonly Member _target;
    private readonly bool _indefinite;
    private readonly int _originalDuration;
    private int _remainingDuration;

    public IStats Stats { get; }
    public StatusTag Tag => StatusTag.DamageOverTime;
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0 || _indefinite;

    public DamageOverTimeState(int amount, Member target, int turns)
    {
        Stats = new StatAddends();
        _amount = amount;
        _target = target;
        _originalDuration = turns;
        _indefinite = turns < 0;
        _remainingDuration = turns;
    }

    public IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        _remainingDuration--;
        _target.State.TakeRawDamage(_amount);
        // TODO: Plug in animations
        return new NoPayload();
    }

    public IPayloadProvider OnTurnEnd() => new NoPayload();
    
    public ITemporalState CloneOriginal() => new DamageOverTimeState(_amount, _target, _originalDuration);
}