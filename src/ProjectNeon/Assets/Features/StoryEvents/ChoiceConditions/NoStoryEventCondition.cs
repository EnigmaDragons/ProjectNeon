using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Condition/None")]
public sealed class NoStoryEventCondition : StoryEventCondition
{
    public override bool Evaluate(StoryEventContext ctx) => true;
}
