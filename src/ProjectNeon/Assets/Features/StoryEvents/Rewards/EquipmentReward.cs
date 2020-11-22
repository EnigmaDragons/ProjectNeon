using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Equipment")]
public class EquipmentReward : StoryResult
{
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private Rarity rarity;
    
    public override void Apply(StoryEventContext ctx)
    {
        var equipment = new EquipmentGenerator().Generate(rarity, slot);
        ctx.Party.Add(equipment);
        Message.Publish(new ShowGainedEquipment(equipment));
    }
}
