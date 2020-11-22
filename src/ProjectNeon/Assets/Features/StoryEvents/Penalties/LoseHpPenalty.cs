using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Penalties/LoseHp")]
public class LoseHpPenalty : StoryResult
{
    [SerializeField] private int minLoss;
    [SerializeField] private int maxLoss;
    [SerializeField] private bool appliesToAll;
    
    public override void Apply(StoryEventContext ctx)
    {
        var members = appliesToAll ? ctx.Party.Heroes : ctx.Party.Heroes.Random().AsArray();
        var amount = -Rng.Int(minLoss, maxLoss);
        members.ForEach(m => m.AdjustHp(amount));
        Message.Publish(new ShowStoryEventResultMessage($"{string.Join(", ", members.Select(m => m.Name))} lost {-amount} HP"));
    }
}
