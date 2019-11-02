
/**
 * Wraps the execution of an effect so it is only executed for a certain amount of turns.
 */
public class ForNumberOfTurns : ITemporalState, Effect
{
    
    private int _remainingDuration;

    private Effect _effect;

    private bool _isDebuff;

    public IStats Stats { get; } = new StatAddends();
    public bool IsDebuff => _isDebuff;
    public bool IsActive => _remainingDuration > 0;

    public void AdvanceTurn()
    {
        if (_remainingDuration <= 0) return;

        _remainingDuration--;
        
    }

    public ForNumberOfTurns(Effect effect, int duration, bool isDebuff)
    {
        _effect = effect;
        _remainingDuration = duration;
        _isDebuff = isDebuff;
    }

    public ForNumberOfTurns(Effect effect, int duration) : this(effect, duration, true) { }

    public ForNumberOfTurns(Effect effect) : this(effect, 1, true) { }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}