using System;

[Serializable]
public class StoryEventChoice
{
    public string Text;
    public StoryEventCondition OptionalCondition;
    public StoryResolution[] resolution;

    public bool CanSelect(StoryEventContext ctx) => OptionalCondition != null ? OptionalCondition.Evaluate(ctx) : true;

    public void Select(StoryEventContext ctx)
    {
        var roll = Rng.Dbl();
        
    }
}
