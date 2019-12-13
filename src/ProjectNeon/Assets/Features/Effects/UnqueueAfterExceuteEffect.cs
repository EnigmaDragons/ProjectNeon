
/**
 * Unqueued the decorated effect from BattleState effect queue after executing.
 * 
 */
public class UnqueueAfterExceuteEffect : Effect
{
    private Effect _effect;

    public UnqueueAfterExceuteEffect(Effect effect)
    {
        _effect = effect;
    }

    public void Apply(Member source, Target target)
    {
        _effect.Apply(source, target);
        BattleEvent.Publish(new EffectApplied(_effect));
    }
}
