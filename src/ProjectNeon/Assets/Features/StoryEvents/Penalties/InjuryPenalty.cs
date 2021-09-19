using UnityEngine;
using UnityEngine.SocialPlatforms;

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
        Message.Publish(new ShowStoryEventResultMessage(Localize.GetFormattedEventResult("InjuryPenalty", hero.DisplayName, injuryName.Value, amount, Localize.GetStat(stat.Value))));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = false, Text = Localize.GetFormattedEventResult("InjuryPenaltyPreview", injuryName.Value, amount, Localize.GetStat(stat.Value))});
    }
}