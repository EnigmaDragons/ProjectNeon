
/**
 * Wraps the execution of an effect so it is only good to execute after the next card is played.
 * 
 * Turn ending makes this effect active. 
 */
public class OnNextCardEffect : Effect
{
    private Effect _effect;
    private bool _active;

    public bool IsActive => _active;

    private void NextCard()
    {
        _active = true;
        Message.Unsubscribe(this);
    }

    public OnNextCardEffect(Effect effect)
    {
        _effect = effect;
        _active = false;
        Message.Subscribe<CardResolutionStarted>((resolutionEnded) => NextCard(), this);
    }

    public void Apply(Member source, Target target)
    {
        if (IsActive)
            _effect.Apply(source, target);
    }
}
