
using System;

public sealed class FeedOnEffect : ConditionalEffect
{
    private Effect _origin;
    private FeedType _type;

    public FeedOnEffect(Effect origin, FeedType feedType) : base(origin)
    {
        _type = feedType;
    }

    public FeedOnEffect(Effect origin, string attribute) : base(origin)
    {
        _type = (FeedType)Enum.Parse(typeof(FeedType), attribute);
    }

    public override bool Condition()
    {
        return Source.State.FeedType == _type;
    }
}