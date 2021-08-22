using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Heat")]
public class HeatResult : StoryResult
{
    [SerializeField] private int adjustment;

    public override int EstimatedCreditsValue => adjustment * -10;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Map.AdjustHeat(adjustment);
        Message.Publish(new ShowStoryEventResultMessage(IsReward ? $"The boss's heat bar has lowered by {-adjustment}" : $"The boss's heat bar has raised by {adjustment}"));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = IsReward ? $"Lower the boss's heat bar by {-adjustment}" : $"Raises the boss's heat bar by {adjustment}" });
    }
}