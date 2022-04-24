using System;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Corp Cost Modifier")]
public class CorpCostModifierResult : StoryResult
{
    [SerializeField] private StringReference corp;
    [SerializeField] private bool appliesToEquipmentShop;
    [SerializeField] private bool appliesToClinic;
    [SerializeField] private float adjustment;

    public override int EstimatedCreditsValue => (int)Math.Ceiling(appliesToEquipmentShop && appliesToClinic 
        ? adjustment * -300f
        : appliesToEquipmentShop 
            ? adjustment * -200f
            : adjustment * -100f);
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Party.AddCorpCostModifier(new CorpCostModifier { Corp = corp.Value, AppliesToEquipmentShop = appliesToEquipmentShop, AppliesToClinic = appliesToClinic, CostPercentageModifier = adjustment });
        if (appliesToEquipmentShop && appliesToClinic && IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-GearClinicReward"), corp.Value, adjustment * -100)));
        else if (appliesToEquipmentShop && appliesToClinic && !IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-GearClinicPenalty"), corp.Value, adjustment * 100)));
        else if (appliesToEquipmentShop && !appliesToClinic && IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-GearReward"), corp.Value, adjustment * -100)));
        else if (appliesToEquipmentShop && !appliesToClinic && !IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-GearPenalty"), corp.Value, adjustment * 100)));
        else if (!appliesToEquipmentShop && appliesToClinic && IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-ClinicReward"), corp.Value, adjustment * -100)));
        else if (!appliesToEquipmentShop && appliesToClinic && !IsReward)
            Message.Publish(new ShowStoryEventResultMessage(string.Format(new LocalizedString("CorpCostModifierResult-ClinicPenalty"), corp.Value, adjustment * 100)));
    }

    public override void Preview()
    {
        if (appliesToEquipmentShop && appliesToClinic && IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
                Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-GearClinicReward"), corp.Value, adjustment * -100)});
        else if (appliesToEquipmentShop && appliesToClinic && !IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
                Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-GearClinicPenalty"), corp.Value, adjustment * 100)});
        else if (appliesToEquipmentShop && !appliesToClinic && IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
                Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-GearReward"), corp.Value, adjustment * -100)});
        else if (appliesToEquipmentShop && !appliesToClinic && !IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, 
                Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-GearPenalty"), corp.Value, adjustment * 100)});
        else if (!appliesToEquipmentShop && appliesToClinic && IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-ClinicReward"), corp.Value, adjustment * -100)});
        else if (!appliesToEquipmentShop && appliesToClinic && !IsReward)
            Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = string.Format(new LocalizedString("CorpCostModifierResultPreview-ClinicPenalty"), corp.Value, adjustment * 100)});
    }
}