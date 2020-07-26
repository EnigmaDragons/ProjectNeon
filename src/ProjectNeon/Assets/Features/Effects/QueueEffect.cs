
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
        Message.Publish(new AddEffectToQueue(effect));
        Message.Subscribe<EffectApplied>(
            (msg) => {
                if (msg.Effect.Equals(_effect))
                {
                    Message.Publish(new RemoveEffectFromQueue(msg.Effect));
                    Message.Unsubscribe(this);
                }
            }, 
        this);
    }
}
