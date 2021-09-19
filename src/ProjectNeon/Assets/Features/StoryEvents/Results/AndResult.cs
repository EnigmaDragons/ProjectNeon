using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "StoryEvent/Results/And")]
public class AndResult : StoryResult
{
    [SerializeField] private StoryResult first;
    [SerializeField] private StoryResult second;
    [SerializeField] private string previewText;
    
    public override int EstimatedCreditsValue => first.EstimatedCreditsValue + second.EstimatedCreditsValue;

    public override void Apply(StoryEventContext ctx)
    {
        first.Apply(ctx);
        second.Apply(ctx);
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
            Text = Localize.GetEventResult(previewText) });
    }
}