
/**
 * Wraps the execution of an effect so it is only executed for a certain amount of turns.
 * 
 * Turn ending decrease the remainingDuration in one. 
 */
public class ForNumberOfTurns : Effect
{
    
    private int _remainingDuration;

    private Effect _effect;

    public bool IsActive => _remainingDuration >= 0;

    private void AdvanceTurn()
    {
        if (_remainingDuration < 0) return;
            _remainingDuration--;
    }

    public ForNumberOfTurns(Effect effect, int duration)
    {
        _effect = effect;
        _remainingDuration = duration;
        BattleEvent.Subscribe<TurnEnd>((turnEnd) => AdvanceTurn(), this);
    }

    public ForNumberOfTurns(Effect effect) : this(effect, 1) { }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}