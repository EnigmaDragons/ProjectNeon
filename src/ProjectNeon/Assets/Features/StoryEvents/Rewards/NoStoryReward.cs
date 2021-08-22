using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/NoReward")]
public class NoStoryReward : StoryResult
{
    public override int EstimatedCreditsValue => 0;
    
    public override void Apply(StoryEventContext ctx) {}
    public override void Preview() {}
}
