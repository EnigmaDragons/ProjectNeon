using System;
using System.Linq;
using I2.Loc;
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
            ? (string)new LocalizedString("PermanentStatAdjustmentResult-All")
            : string.Format(new LocalizedString("PermanentStatAdjustmentResult-Single"), members.First().DisplayName);
        Message.Publish(new ShowStoryEventResultMessage($"{baseMessage}\n{string.Join("\n", permanentStatAdjustments.Select(x => $"{x.Amount} { new LocalizedString(x.Stat.ToString())}"))}"));
    }

    public override void Preview()
    {
        var baseMessage = appliesToAll
            ? new LocalizedString("PermanentStatAdjustmentResultPreview-All")
            : new LocalizedString("PermanentStatAdjustmentResultPreview-Single");
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
            Text = $"{baseMessage}\n{string.Join("\n", permanentStatAdjustments.Select(x => $"{x.Amount} {x.Stat}"))}" });
    }
}