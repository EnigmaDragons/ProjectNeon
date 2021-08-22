using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Permanent Stat Adjustment")]
public class PermanentStatAdjustmentResult : StoryResult
{
    [SerializeField] private PermenatStatAdjustmentData[] permanentStatAdjustments;
    [SerializeField] private int estimatedCreditsValue;
    [SerializeField] private bool isReward;
    [SerializeField] private bool appliesToAll;

    public override int EstimatedCreditsValue => estimatedCreditsValue;
    public override bool IsReward => isReward;
    
    public override void Apply(StoryEventContext ctx)
    {
        var members = appliesToAll ? ctx.Party.Heroes : ctx.Party.Heroes.Random().AsArray();
        foreach (var stat in permanentStatAdjustments)
        {
            foreach (var member in members)
            {
                if (member.Character.Stats[stat.Stat] > 0)
                    member.ApplyPermanent(new InMemoryEquipment
                    {
                        Name = "Implant",
                        Slot = EquipmentSlot.Permanent,
                        Modifiers = new [] {new EquipmentStatModifier {Amount = stat.Amount, StatType = stat.Stat.ToString(), ModifierType = StatMathOperator.Additive}}
                    });
            }
        }
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = $"{(string.Join(" & ", members.Select(x => x.Character.Name)))} permanently {string.Join(" & ", permanentStatAdjustments.Select(x => $"{(x.Amount > 0 ? "gained" : "lost")} {Math.Abs(x.Amount)} {x.Stat}"))} if they had any" });
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = $"{(appliesToAll ? "All heroes" : "A random hero")} permanently {string.Join(" & ", permanentStatAdjustments.Select(x => $"{(x.Amount > 0 ? "gain" : "lose")} {Math.Abs(x.Amount)} {x.Stat}"))} if they have any" });
    }
}