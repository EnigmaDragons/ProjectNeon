
public class DamageOverTimeState : TemporalStateBase
{
    private readonly int _amount;
    private readonly Member _target;

    public DamageOverTimeState(int amount, Member target, int turns)
        : this(amount, target, TemporalStateMetadata.DebuffForDuration(turns, new StatusDetail(StatusTag.DamageOverTime))) {}
    
    public DamageOverTimeState(int amount, Member target, TemporalStateMetadata metadata)
        : base(metadata) 
    {
        _amount = amount;
        _target = target;
    }

    public override ITemporalState CloneOriginal() => new DamageOverTimeState(_amount, _target, Tracker.Metadata);
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount => _amount;

    public override IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        return new SinglePayload(new PerformAction(() =>
        {
            BattleLog.Write($"{_amount} DoT damage dealt to {_target.Name}");
            _target.State.TakeRawDamage(_amount);
        }));
    }

    public override IPayloadProvider OnTurnEnd() => new NoPayload();
}
