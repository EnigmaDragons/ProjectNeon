using UnityEngine;

public abstract class StoryEventCondition : ScriptableObject
{
    public abstract bool Evaluate(StoryEventContext ctx);
}
