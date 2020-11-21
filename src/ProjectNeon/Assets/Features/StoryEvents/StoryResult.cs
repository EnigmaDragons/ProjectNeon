using UnityEngine;

public abstract class StoryResult : ScriptableObject
{
    public abstract void Apply(StoryEventContext ctx);
}
