using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Credits")]
public class CreditsReward : StoryResult
{
    [SerializeField] private int minCredits;
    [SerializeField] private int maxCredits;
    
    public override void Apply(StoryEventContext ctx)
    {
        var amount = Rng.Int(minCredits, maxCredits);
        ctx.Party.UpdateCreditsBy(amount);
        Message.Publish(new ShowCreditChange(amount));
    }
}
