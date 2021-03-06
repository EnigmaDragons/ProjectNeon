using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class StoryEventChoice
{
    public string Text;
    public StoryEventCost OptionalCost;
    public StoryEventCondition OptionalCondition;
    public StoryResolution[] Resolution;

    public string ChoiceFullText(StoryEventContext ctx)
    {
        var sb = new StringBuilder();
        sb.Append(Text.Trim());
        if (OptionalCondition != null)
            sb.Append($" {OptionalCondition.ConditionDescription}");
        if (OptionalCost.CostAmount > 0)
            sb.Append($" {OptionalCost.CostDescription()}");
        return sb.ToString();
    }

    public bool CanSelect(StoryEventContext ctx) => ConditionMet(ctx) && CostIsAffordable(ctx);

    private bool ConditionMet(StoryEventContext ctx) => OptionalCondition != null ? OptionalCondition.Evaluate(ctx) : true;
    private bool CostIsAffordable(StoryEventContext ctx) => OptionalCost != null ? OptionalCost.CanAfford(ctx) : true;

    public void Select(StoryEventContext ctx)
    {
        OptionalCost?.Apply(ctx);

        if (Resolution.Sum(r => r.Chance) > 1 || Resolution.Sum(r => r.Chance) <= 0)
        {
            Log.Error($"Story Event: Invalid Total Resolution Chance for {Text}");
            Message.Publish(new ShowStoryEventResolution("Something peculiar occurred, which you can't explain, of which you can never speak (except to the developers)"));
        }

        var roll = Rng.Dbl();
        if (Resolution.Length > 1)
            Message.Publish(new ShowDieRoll((int)Math.Ceiling(Math.Abs(1 - roll) * 20)));
        else if (Resolution.Length == 1 && !Resolution.Single().HasContinuation)
            Message.Publish(new ShowNoDieRollNeeded());
        var possibleOutcomes = new Dictionary<float, StoryResolution>();
        var rangeStart = 0f;
        foreach (var r in Resolution.OrderByDescending(x => x.EstimatedCreditsValue))
        {
            possibleOutcomes[rangeStart + r.Chance] = r;
            rangeStart += r.Chance;
        }
        ResolveSelectedResolution(possibleOutcomes.First(x => roll < x.Key).Value, ctx);
    }

    private void ResolveSelectedResolution(StoryResolution r, StoryEventContext ctx)
    {
        if (r.HasContinuation)
        {
            Message.Publish(new BeginStoryEvent(r.ContinueWith));
        }
        else
        {
            r.Result.Apply(ctx);
            Message.Publish(new ShowStoryEventResolution(r.StoryText));   
        }
    }
}
