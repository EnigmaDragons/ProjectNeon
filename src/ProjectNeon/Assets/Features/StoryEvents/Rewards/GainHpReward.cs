using System.Linq;
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
        Message.Publish(new ShowStoryEventResultMessage($"{string.Join(", ", members.Select(m => m.Name))} gained {amount} HP"));
    }
}