using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/Injury")]
public class InjuryPenalty : StoryResult
{
    [SerializeField] private StringReference stat;
    [SerializeField] private float amount;
    [SerializeField] private StringReference injuryName;
    [SerializeField] private int estimatedCreditsValue;

    public override int EstimatedCreditsValue => estimatedCreditsValue;
    public override bool IsReward => false;
    
    public override void Apply(StoryEventContext ctx)
    {
        var hero = ctx.Party.Heroes.Random();
        hero.Apply(new AdditiveStatInjury {Stat = stat, Amount = amount, Name = injuryName});
        Message.Publish(new ShowStoryEventResultMessage($"{hero.Name} gained the injury \"{injuryName.Value}\": {amount} {stat.Value}"));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { Text = $"A hero gains the injury \"{injuryName.Value}\": {amount} {stat.Value}", IsReward = false });
    }
}