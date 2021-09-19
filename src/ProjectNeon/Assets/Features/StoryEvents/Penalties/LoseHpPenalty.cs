using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/LoseHp")]
public class LoseHpPenalty : StoryResult
{
    [SerializeField] private int minLoss;
    [SerializeField] private int maxLoss;
    [SerializeField] private bool appliesToAll;

    public override int EstimatedCreditsValue => -(maxLoss + minLoss / 2) * (appliesToAll ? 3 : 1);
    
    public override void Apply(StoryEventContext ctx)
    {
        var members = appliesToAll ? ctx.Party.Heroes : ctx.Party.Heroes.Random().AsArray();
        var amount = -Rng.Int(minLoss, maxLoss);
        members.ForEach(m => m.AdjustHp(amount));
        Message.Publish(new ShowStoryEventResultMessage(appliesToAll
            ? Localize.GetFormattedEventResult("LoseHpPenalty-All", -amount)
            : Localize.GetFormattedEventResult("LoseHpPenalty-Single", members.First().Name, -amount)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview
            {
                IsReward = false, 
                Text = appliesToAll 
                    ? Localize.GetFormattedEventResult("LoseHpPenaltyPreview-All", minLoss, maxLoss)
                    : Localize.GetFormattedEventResult("LoseHpPenaltyPreview-Single", minLoss, maxLoss)
            });
    }
}
