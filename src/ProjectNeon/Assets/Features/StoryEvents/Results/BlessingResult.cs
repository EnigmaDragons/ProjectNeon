using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Blessing")]
public class BlessingResult : StoryResult
{
    [SerializeField] private int estimatedCreditsValue;
    [SerializeField] private bool isReward;
    [SerializeField] private string text;
    [SerializeField] private string previewText;
    [SerializeField] private Blessing curse;
    public override int EstimatedCreditsValue => estimatedCreditsValue;
    public override bool IsReward => isReward;
    public override void Apply(StoryEventContext ctx)
    {
        curse.Targets = ctx.Party.BaseHeroes;
        ctx.Party.AddBlessing(curse);
        Message.Publish(new ShowStoryEventResultMessage(text));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = isReward, Text = previewText });
    }
}