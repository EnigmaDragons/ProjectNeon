using UnityEngine;

public abstract class StoryResult : ScriptableObject
{
    public abstract int EstimatedCreditsValue { get; }
    public abstract bool IsReward { get; }
    public abstract void Apply(StoryEventContext ctx);
    public abstract void Preview();
}
