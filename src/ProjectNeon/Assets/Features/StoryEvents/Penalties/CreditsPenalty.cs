using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/Credits")]
public class CreditsPenalty : StoryResult
{
    [SerializeField] private int minCredits;
    [SerializeField] private int maxCredits;

    public override int EstimatedCreditsValue => -(maxCredits + minCredits / 2);
    
    public override void Apply(StoryEventContext ctx)
    {
        var amount = -Rng.Int(minCredits, maxCredits);
        ctx.Party.UpdateCreditsBy(amount);
        Message.Publish(new ShowCreditChange(amount));
    }

    public override void Preview()
    {
        Message.Publish(new ShowCredResultPreview { Creds = EstimatedCreditsValue, IsReward = false });
    }
}
