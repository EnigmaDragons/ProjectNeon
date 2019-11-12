
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
        TemporalStatType type = (TemporalStatType)Enum.Parse(typeof(TemporalStatType), _attribute);
        return _source.State[type] == 1;
    }
}