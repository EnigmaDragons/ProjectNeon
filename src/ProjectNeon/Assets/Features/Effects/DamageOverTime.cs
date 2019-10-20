
public sealed class DamageOverTime : ITemporalState, Effect
{
    private readonly int _amount;
    private Target _target;
    private int _remainingDuration;
    
    public IStats Stats { get; } = new StatAddends();
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0;
    public void AdvanceTurn()
    {
        if (_remainingDuration <= 0) return;
        
        _remainingDuration--;
        _target.ApplyToAll(m => m.TakeRawDamage(_amount));
    }

    public DamageOverTime(EffectData data)
    {
        _amount = data.IntAmount;
        _remainingDuration = data.NumberOfTurns;
    }

    public void Apply(Member source, Target target)
    {
        _target = target;
    }
}
