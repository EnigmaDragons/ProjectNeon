using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Blessing")]
public class NextCombatEffectResult : StoryResult
{
    [SerializeField] private int estimatedCreditsValue;
    [SerializeField] private string text;
    [SerializeField] private string previewText;
    [SerializeField] private Blessing nextCombatEffect;
    
    public override int EstimatedCreditsValue => estimatedCreditsValue;
    
    public override void Apply(StoryEventContext ctx)
    {
        nextCombatEffect.Targets = ctx.Party.BaseHeroes;
        ctx.Party.AddBlessing(nextCombatEffect);
        Message.Publish(new ShowStoryEventResultMessage(Localize.GetEventResult(text)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = estimatedCreditsValue > 0, Text = Localize.GetEventResult(previewText) });
    }
}