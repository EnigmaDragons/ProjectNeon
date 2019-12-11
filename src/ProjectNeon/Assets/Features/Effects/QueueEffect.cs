
/**
 * Queues the decorated effect in BattleState effect queue.
 * 
 */
public class QueueEffect : Effect
{
    private Effect _effect;

    public QueueEffect(Effect effect)
    {
        _effect = effect;
    }

    public void Apply(Member source, Target target)
    {
        BattleEvent.Publish(new AddEffectToQueue(_effect));
    }
}
