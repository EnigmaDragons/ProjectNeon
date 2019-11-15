
using System;
using UnityEngine;

public sealed class NotFeedOnEffect : ConditionalEffect
{
    private Effect _origin;
    private FeedType _type;

    public NotFeedOnEffect(Effect origin, FeedType feedType) : base(origin)
    {
        _type = feedType;
    }

    public NotFeedOnEffect(Effect origin, string attribute) : base(origin)
    {
        _type = (FeedType)Enum.Parse(typeof(FeedType), attribute);
    }

    public override bool Condition()
    {
        return !Source.State.Status().ContainsKey("Feed") || Source.State["Feed"] != Enum.GetName(typeof(FeedType), _type);
    }
}