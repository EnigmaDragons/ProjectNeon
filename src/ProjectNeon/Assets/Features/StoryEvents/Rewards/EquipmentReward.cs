using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Equipment")]
public class EquipmentReward : StoryResult
{
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private Rarity rarity;

    public override int EstimatedCreditsValue => rarity.EquipmentShopPrice(1f);

    public override void Apply(StoryEventContext ctx)
    {
        var equipment = ctx.EquipmentPool.Random(slot, rarity, ctx.Party.BaseHeroes);
        ctx.Party.Add(equipment);
        Message.Publish(new ShowGainedEquipment(equipment));
    }
}
