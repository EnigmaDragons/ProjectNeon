using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/StaticEquipment")]
public class StaticEquipmentReward : StoryResult
{
    [SerializeField] private StaticEquipment equipment;

    public override int EstimatedCreditsValue => equipment.Price;
    public override bool IsReward => true;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Party.Add(equipment);
        Message.Publish(new ShowGainedEquipment(equipment));
    }

    public override void Preview()
    {
        Message.Publish(new ShowEquipmentResultPreview { Equipment = equipment });
    }
}