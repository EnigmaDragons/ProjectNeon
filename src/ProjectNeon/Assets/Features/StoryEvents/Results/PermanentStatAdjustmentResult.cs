using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(menuName = "StoryEvent/Results/Permanent Stat Adjustment")]
public class PermanentStatAdjustmentResult : StoryResult
{
    [SerializeField] private PermenatStatAdjustmentData[] permanentStatAdjustments;
    [SerializeField] private int estimatedCreditsValue;
    [SerializeField] private bool appliesToAll;

    public override int EstimatedCreditsValue => estimatedCreditsValue;
    
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
        var baseMessage = appliesToAll
            ? Localize.GetEventResult("PermanentStatAdjustmentResult-All")
            : Localize.GetFormattedEventResult("PermanentStatAdjustmentResult-Single", members.First().DisplayName);
        Message.Publish(new ShowStoryEventResultMessage($"{baseMessage}\n{string.Join("\n", permanentStatAdjustments.Select(x => $"{x.Amount} {Localize.GetStat(x.Stat)}"))}"));
    }

    public override void Preview()
    {
        var baseMessage = appliesToAll
            ? Localize.GetEventResult("PermanentStatAdjustmentResultPreview-All")
            : Localize.GetEventResult("PermanentStatAdjustmentResultPreview-Single");
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
            Text = $"{baseMessage}\n{string.Join("\n", permanentStatAdjustments.Select(x => $"{x.Amount} {x.Stat}"))}" });
    }
}