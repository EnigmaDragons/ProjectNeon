using System.Linq;
using I2.Loc;
using UnityEngine;

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
            ? string.Format(new LocalizedString("LoseHpPenalty-All"), -amount)
            : string.Format(new LocalizedString("LoseHpPenalty-Single"), members.First().NameTerm.ToEnglish(), -amount)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview
            {
                IsReward = false, 
                Text = appliesToAll 
                    ? string.Format(new LocalizedString("LoseHpPenaltyPreview-All"), minLoss, maxLoss)
                    : string.Format(new LocalizedString("LoseHpPenaltyPreview-Single"), minLoss, maxLoss)
            });
    }
}
