using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "StoryEvent/Results/Heat")]
public class HeatResult : StoryResult
{
    [SerializeField] private int adjustment;

    public override int EstimatedCreditsValue => adjustment * -10;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Map.AdjustHeat(adjustment);
        Message.Publish(new ShowStoryEventResultMessage(IsReward 
            ? Localize.GetFormattedEventResult("HeatResult-Reward", -adjustment)
            : Localize.GetFormattedEventResult("HeatResult-Penalty", adjustment)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = IsReward
                ? Localize.GetFormattedEventResult("HeatResultPreview-Reward", -adjustment)
                : Localize.GetFormattedEventResult("HeatResultPreview-Penalty", adjustment)});
    }
}