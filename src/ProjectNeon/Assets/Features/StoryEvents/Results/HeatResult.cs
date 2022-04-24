using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Heat")]
public class HeatResult : StoryResult
{
    [SerializeField] private int adjustment;

    public override int EstimatedCreditsValue => adjustment * -10;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Map.AdjustHeat(adjustment);
        Message.Publish(new ShowStoryEventResultMessage(IsReward 
            ? string.Format(new LocalizedString("HeatResult-Reward"), -adjustment)
            : string.Format(new LocalizedString("HeatResult-Penalty"), adjustment)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = IsReward
                ? string.Format(new LocalizedString("HeatResultPreview-Reward"), -adjustment)
                : string.Format(new LocalizedString("HeatResultPreview-Penalty"), adjustment)});
    }
}