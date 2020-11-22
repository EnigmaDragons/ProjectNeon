using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StoryEventChoice
{
    public string Text;
    public StoryEventCondition OptionalCondition;
    public StoryResolution[] Resolution;

    public bool CanSelect(StoryEventContext ctx) => OptionalCondition != null ? OptionalCondition.Evaluate(ctx) : true;

    public void Select(StoryEventContext ctx)
    {
        if (Resolution.Sum(r => r.Chance) > 1 || Resolution.Sum(r => r.Chance) > 1)
            Log.Error($"Story Event: Invalid Total Resolution Chance for {Text}");
        
        var roll = Rng.Dbl();
        var possibleOutcomes = new Dictionary<float, StoryResolution>();
        var rangeStart = 0f;
        foreach (var r in Resolution)
        {
            possibleOutcomes[rangeStart + r.Chance] = r;
            rangeStart += r.Chance;
        }
        ResolveSelectedResolution(possibleOutcomes.First(x => roll < x.Key).Value, ctx);
    }

    private void ResolveSelectedResolution(StoryResolution r, StoryEventContext ctx)
    {
        r.Result.Apply(ctx);
        Message.Publish(new ShowStoryEventResolution(r.StoryText));
    }
}
