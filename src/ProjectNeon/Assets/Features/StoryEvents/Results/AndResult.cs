using System;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/And")]
public class AndResult : StoryResult
{
    [SerializeField] private StoryResult first;
    [SerializeField] private StoryResult second;
    [SerializeField] private bool isReward;
    [SerializeField] private string text;
    [SerializeField] private string previewText;
    public override int EstimatedCreditsValue => first.EstimatedCreditsValue + second.EstimatedCreditsValue;
    public override bool IsReward => isReward;

    public override void Apply(StoryEventContext ctx)
    {
        first.Apply(ctx);
        second.Apply(ctx);
        Message.Publish(new ShowStoryEventResultMessage(text));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { Text = previewText, IsReward = isReward });
    }
}