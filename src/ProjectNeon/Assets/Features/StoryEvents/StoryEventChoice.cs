using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StoryEventChoice
{
    public string Text;
    public StoryEventCost OptionalCost;
    public StoryEventCondition OptionalCondition;
    public StoryResolution[] Resolution;

    public string ChoiceFullText(StoryEventContext ctx) => $"{Text} {OptionalCost?.CostDescription()}".Trim();
    
    public bool CanSelect(StoryEventContext ctx) => ConditionMet(ctx) && CostIsAffordable(ctx);

    private bool ConditionMet(StoryEventContext ctx) => OptionalCondition != null ? OptionalCondition.Evaluate(ctx) : true;
    private bool CostIsAffordable(StoryEventContext ctx) => OptionalCost != null ? OptionalCost.CanAfford(ctx) : true;

    public void Select(StoryEventContext ctx)
    {
        OptionalCost?.Apply(ctx);

        if (Resolution.Sum(r => r.Chance) > 1 || Resolution.Sum(r => r.Chance) > 1)
        {
            Log.Error($"Story Event: Invalid Total Resolution Chance for {Text}");
            Message.Publish(new ShowStoryEventResolution("Something peculiar occurred, which you can't explain, of which you can never speak (except to the developers)"));
        }

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
