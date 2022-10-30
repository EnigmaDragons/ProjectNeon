using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/Injury")]
public class InjuryPenalty : StoryResult
{
    [SerializeField] private StringReference stat;
    [SerializeField] private float amount;
    [SerializeField] private StringReference injuryName;
    [SerializeField] private int estimatedCreditsValue;

    public override int EstimatedCreditsValue => estimatedCreditsValue;
    
    public override void Apply(StoryEventContext ctx)
    {
        var hero = ctx.Party.Heroes.Random();
        hero.Apply(new AdditiveStatInjury {Stat = stat, Amount = amount, Name = injuryName});
        Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("InjuryPenalty"), hero.NameTerm.ToEnglish(), injuryName.Value, amount, new LocalizedString(stat.Value))));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = false, Text = string.Format(new LocalizedString("InjuryPenaltyPreview"), injuryName.Value, amount, new LocalizedString(stat.Value))});
    }
}