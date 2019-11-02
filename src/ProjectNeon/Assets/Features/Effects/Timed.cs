
/**
 * Wraps the execution of an effect so it is only executed for a certain amount of turns.
 */
public class Timed : ITemporalState, Effect
{
    
    private int _remainingDuration;

    private Effect _effect;

    public IStats Stats { get; } = new StatAddends();
    public bool IsDebuff => true;
    public bool IsActive => _remainingDuration > 0;

    public void AdvanceTurn()
    {
        if (_remainingDuration <= 0) return;

        _remainingDuration--;
        
    }

    public Timed(Effect effect, int duration)
    {
        _effect = effect;
        _remainingDuration = duration;
    }

    public Timed(Effect effect) : this(effect, 1) { }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}