using System;

[Serializable]
public class StoryEventChoice
{
    public string Text;
    public StoryEventCondition OptionalCondition;
    public StoryResolution[] Resolution;

    public bool CanSelect(StoryEventContext ctx) => OptionalCondition != null ? OptionalCondition.Evaluate(ctx) : true;

    public void Select(StoryEventContext ctx)
    {
        if (Resolution.Length == 1 && Math.Abs(Resolution[0].Chance - 1f) < 0.01)
        {
            ResolveSelectedResolution(Resolution[0], ctx);
        }
    }

    private void ResolveSelectedResolution(StoryResolution r, StoryEventContext ctx)
    {
        r.Result.Apply(ctx);
        Message.Publish(new ShowStoryEventResolution(r.StoryText));
    }
}
