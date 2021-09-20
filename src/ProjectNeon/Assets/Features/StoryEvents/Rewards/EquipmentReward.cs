using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Equipment")]
public class EquipmentReward : StoryResult
{
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private Rarity rarity;
    [SerializeField] private StringReference corp;

    public override int EstimatedCreditsValue => rarity.EquipmentShopPrice(1f);
    
    public override void Apply(StoryEventContext ctx)
    {
        var equipment = ctx.EquipmentPool.Random(slot, rarity, ctx.Party.BaseHeroes, corp.Value);
        ctx.Party.Add(equipment);
        Message.Publish(new ShowGainedEquipment(equipment));
    }
    
    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = true, Text = string.IsNullOrWhiteSpace(corp.Value) 
            ? Localize.GetFormattedEventResult("EquipmentRewardPreview", rarity, slot)
            : Localize.GetFormattedEventResult("EquipmentRewardPreview-Corp", rarity, slot, corp.Value)});
    }
}
