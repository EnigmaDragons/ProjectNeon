
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
        Effect effect = new UnqueueAfterExceuteEffect(_effect);
        BattleEvent.Publish(new AddEffectToQueue(effect));
        BattleEvent.Subscribe<EffectApplied>(
            (msg) => {
                if (msg.Effect.Equals(_effect))
                {
                    BattleEvent.Publish(new RemoveEffectFromQueue(msg.Effect));
                    BattleEvent.Unsubscribe(this);
                }
            }, 
        this);
    }
}
