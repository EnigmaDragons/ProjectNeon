using System;
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
    public override bool IsReward => adjustment < 0;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Party.AddCorpCostModifier(new CorpCostModifier { Corp = corp.Value, AppliesToEquipmentShop = appliesToEquipmentShop, AppliesToClinic = appliesToClinic, CostPercentageModifier = adjustment });
        Message.Publish(new ShowStoryEventResultMessage($"{corp.Value}'s{(appliesToEquipmentShop ? " equipment shop" : "")}{(appliesToEquipmentShop && appliesToClinic ? " and" : "")}{(appliesToClinic ? " clinic" : "")} are now {(IsReward ? adjustment * -100 : adjustment * 100)}% more {(IsReward ? "cheap" : "expensive")}"));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = IsReward, Text = $"{corp.Value}'s{(appliesToEquipmentShop ? " equipment shop" : "")}{(appliesToEquipmentShop && appliesToClinic ? " and" : "")}{(appliesToClinic ? " clinic" : "")} will be {(IsReward ? adjustment * -100 : adjustment * 100)}% more {(IsReward ? "cheap" : "expensive")}" });
    }
}