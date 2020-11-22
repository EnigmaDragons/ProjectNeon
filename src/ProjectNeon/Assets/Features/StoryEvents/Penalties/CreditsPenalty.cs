using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/Credits")]
public class CreditsPenalty : StoryResult
{
    [SerializeField] private int minCredits;
    [SerializeField] private int maxCredits;
    
    public override void Apply(StoryEventContext ctx)
    {
        var amount = -Rng.Int(minCredits, maxCredits);
        ctx.Party.UpdateCreditsBy(amount);
        Message.Publish(new ShowCreditChange(amount));
    }
}
