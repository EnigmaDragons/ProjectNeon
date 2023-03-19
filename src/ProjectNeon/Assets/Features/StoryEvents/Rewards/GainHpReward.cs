using System.Linq;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Hp")]
public class GainHpReward : StoryResult
{
    [SerializeField] private int minGain;
    [SerializeField] private int maxGain;
    [SerializeField] private bool appliesToAll;

    public override int EstimatedCreditsValue => maxGain + minGain / 2 * (appliesToAll ? 3 : 1);
    
    public override void Apply(StoryEventContext ctx)
    {
        var members = appliesToAll ? ctx.Party.Heroes : ctx.Party.Heroes.Random().AsArray();
        var amount = Rng.Int(minGain, maxGain);
        members.ForEach(m => m.AdjustHp(amount));
        Message.Publish(new ShowStoryEventResultMessage(appliesToAll
            ? string.Format(new LocalizedString("GainHpReward-All"), amount)
            : string.Format(new LocalizedString("GainHpReward-Single"), members.First().NameTerm.ToEnglish(), amount)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = true, Text = appliesToAll 
            ? string.Format(new LocalizedString("GainHpRewardPreview-All"), minGain, maxGain) 
            : string.Format(new LocalizedString("GainHpRewardPreview-Single"), minGain, maxGain)  });
    }
}