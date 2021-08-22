using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Fallback")]
public class FallbackResult : StoryResult
{
    public override int EstimatedCreditsValue { get; }
    public override bool IsReward { get; }
    public override void Apply(StoryEventContext ctx)
    {
        throw new System.NotImplementedException();
    }

    public override void Preview()
    {
        throw new System.NotImplementedException();
    }
}