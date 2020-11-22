using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/Credits")]
public class CreditsPenalty : StoryResult
{
    [SerializeField] private int minCredits;
    [SerializeField] private int maxCredits;
    
    public override void Apply(StoryEventContext ctx) 
        => ctx.Party.UpdateCreditsBy(-Rng.Int(minCredits, maxCredits));
}
