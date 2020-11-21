using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Credits")]
public class CreditsReward : StoryResult
{
    [SerializeField] private int minCredits;
    [SerializeField] private int maxCredits;
    
    public override void Apply(StoryEventContext ctx) 
        => ctx.Party.UpdateCreditsBy(Rng.Int(minCredits, maxCredits));
}
