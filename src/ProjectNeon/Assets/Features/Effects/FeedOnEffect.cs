
using System;

public sealed class FeedOnEffect : ConditionalEffect
{
    private Effect _origin;
    private string _attribute;

    public FeedOnEffect(Effect origin, string attribute) : base(origin)
    {
        _attribute = attribute;
    }

    public override bool Condition()
    {
        return Source.State.FeedType == (FeedType)Enum.Parse(typeof(FeedType), _attribute);
    }
}