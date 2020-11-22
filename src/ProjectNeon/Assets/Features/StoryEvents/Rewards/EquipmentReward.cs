using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Equipment")]
public class EquipmentReward : StoryResult
{
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private Rarity rarity;
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Party.Add(new EquipmentGenerator().Generate(rarity, slot));
    }
}
