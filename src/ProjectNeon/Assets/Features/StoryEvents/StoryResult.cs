using UnityEngine;

public abstract class StoryResult : ScriptableObject
{
    public abstract int EstimatedCreditsValue { get; }
    public abstract void Apply(StoryEventContext ctx);
    public abstract void Preview();

    public bool IsReward => EstimatedCreditsValue > 0;
}
