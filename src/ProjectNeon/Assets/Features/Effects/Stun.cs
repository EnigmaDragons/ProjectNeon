
public sealed class Stun : ITemporalState, Effect
{
    private Target _target;
    private int _remainingDuration;
    
    public IStats Stats { get; } = new StatAddends();
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0;
    public void OnTurnStart() {}
    public void OnTurnEnd()
    {
        if (_remainingDuration <= 0) return;
        
        _remainingDuration--;
    }

    public Stun(EffectData data)
    {
        _remainingDuration = data.NumberOfTurns;
    }

    public void Apply(Member source, Target target)
    {
        _target = target;
    }
}
