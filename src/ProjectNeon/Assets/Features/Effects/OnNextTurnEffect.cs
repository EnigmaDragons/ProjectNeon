
/**
 * Wraps the execution of an effect so it is only good to execute after the current turn ends.
 * 
 * Turn ending makes this effect active. 
 */
public class OnNextTurnEffect : Effect
{
    private Effect _effect;
    private bool _active;

    public bool IsActive => _active;

    private void AdvanceTurn()
    {
        _active = true;
        BattleEvent.Unsubscribe(this);
    }

    public OnNextTurnEffect(Effect effect)
    {
        _effect = effect;
        BattleEvent.Subscribe<TurnEnd>((turnEnd) => AdvanceTurn(), this);
    }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}
