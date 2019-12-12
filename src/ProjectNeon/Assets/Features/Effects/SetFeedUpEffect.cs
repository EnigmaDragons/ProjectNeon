
using System;

/**
 * Puts the decorated effect on queue and sets up a trigger so it is executed after
 * the next card is played and card is of some specific type.
 * 
 * When some "feedType" card is played, this effect is triggered. If the "feedType" is
 * different from this card, the effect is wasted.
 * 
 */
public class SetFeedUpEffect : Effect
{
    private Effect _effect;

    /**
     * Feed type that triggers the effect execution
     */
    private FeedType _feedType;

    public SetFeedUpEffect(Effect origin, FeedType feedType)
    {
        _effect = origin;
        _feedType = feedType;
    }

    public SetFeedUpEffect(Effect origin, string attribute) : this(origin, (FeedType)Enum.Parse(typeof(FeedType), attribute))
    {

    }

    public void Apply(Member source, Target target)
    {

        Effect effect = new UnqueueAfterExceuteEffect(_effect);
        BattleEvent.Publish(new AddEffectToQueue(effect));

        BattleEvent.Subscribe<FeedCardResolutionStarted>(
            (msg) =>
            {
                if (Enum.GetName(typeof(FeedType), _feedType) == msg.CardFeedType)
                    _effect.Apply(source, target);
                BattleEvent.Unsubscribe(this);
            },
        this);
    }
}
