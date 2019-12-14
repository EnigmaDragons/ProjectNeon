using System;

/**
 * Represents an effect that triggers feed card logic.
 * 
 * When this effect is played, it will trigger the effeccts queued which have the same type of this.
 * 
 */
public sealed class TriggerFeedEffects : Effect
{
    private Effect _origin;
    private FeedType _type;

    public TriggerFeedEffects(Effect origin, FeedType feedType) 
    {
        _origin = origin;
        _type = feedType;
    }

    public TriggerFeedEffects(Effect origin, string attribute) : this(origin, (FeedType)Enum.Parse(typeof(FeedType), attribute))
    {
        
    }

    public void Apply(Member source, Target target)
    {
        _origin.Apply(source, target);
        BattleEvent.Publish(new FeedCardResolutionStarted(_type.ToString()));
    }
}

