
public class DamageOverTimeState : TemporalStateBase
{
    private readonly int _amount;
    private readonly Member _target;

    public DamageOverTimeState(int amount, Member target, int turns)
        : this(amount, target, TemporalStateMetadata.DebuffForDuration(turns, StatusTag.DamageOverTime)) {}
    
    public DamageOverTimeState(int amount, Member target, TemporalStateMetadata metadata)
        : base(metadata) 
    {
        _amount = amount;
        _target = target;
    }

    public override ITemporalState CloneOriginal() => new DamageOverTimeState(_amount, _target, Tracker.Metadata);
    public override Maybe<string> CustomStatusText { get; } = Maybe<string>.Missing();
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount => _amount;

    public override IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        BattleLog.Write($"{_amount} DoT damage dealt to {_target.Name}");
        _target.State.TakeRawDamage(_amount);
        // TODO: Plug in animations
        return new NoPayload();
    }

    public override IPayloadProvider OnTurnEnd() => new NoPayload();
}
