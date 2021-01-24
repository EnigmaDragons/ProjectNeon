public class HealOverTimeState : TemporalStateBase
{
    private readonly int _amount;
    private readonly Member _target;

    public HealOverTimeState(int amount, Member target, int turns)
        : this(amount, target, TemporalStateMetadata.BuffForDuration(turns, new StatusDetail(StatusTag.HealOverTime))) {}
    
    public HealOverTimeState(int amount, Member target, TemporalStateMetadata metadata)
        : base(metadata) 
    {
        _amount = amount;
        _target = target;
    }

    public override ITemporalState CloneOriginal() => new HealOverTimeState(_amount, _target, Tracker.Metadata);
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount => _amount;

    public override IPayloadProvider OnTurnStart()
    {
        if (!IsActive || !_target.IsConscious()) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        _target.State.GainHp(_amount);
        // TODO: Plug in animations
        return new NoPayload();
    }

    public override IPayloadProvider OnTurnEnd() => new NoPayload();
}
