
/**
 * Wraps the execution of an effect so it is only executed for a certain amount of turns.
 * 
 * Turn ending decrease the remainingDuration in one. 
 */
public class ForNumberOfTurns : Effect
{
    
    private bool _isDebuff;
    private int _remainingDuration;

    private Effect _effect;

    public bool IsActive => _remainingDuration >= 0;

    public bool IsDebuff => _isDebuff;

    private void AdvanceTurn()
    {
        if (_remainingDuration < 0) {
            BattleEvent.Unsubscribe(this);
            return;
        }
        _remainingDuration--;
    }

    public ForNumberOfTurns(Effect effect, int duration, bool isDebuff)
    {
        _effect = effect;
        _remainingDuration = duration;
	_isDebuff = isDebuff;
        BattleEvent.Subscribe<TurnEnd>((turnEnd) => AdvanceTurn(), this);
    }

    public ForNumberOfTurns(Effect effect, int duration) : this(effect, duration, true) { }

    public ForNumberOfTurns(Effect effect) : this(effect, 1, true) { }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}
