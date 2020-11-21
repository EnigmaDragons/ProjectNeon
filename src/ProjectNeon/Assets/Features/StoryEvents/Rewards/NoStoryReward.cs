using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/NoReward")]
public class NoStoryReward : StoryResult
{
    public override void Apply(StoryEventContext ctx) {}
}
