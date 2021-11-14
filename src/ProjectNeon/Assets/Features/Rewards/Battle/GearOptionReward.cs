using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/GearOption")]
public class GearOptionReward : BattleRewards
{
    [SerializeField] private BattleState state;
    [SerializeField] private EquipmentPool equipmentPrizePool;
    
    public override void GrantVictoryRewardsAndThen(Action onFinished, LootPicker lootPicker) 
        => GetUserSelectedEquipment(onFinished, lootPicker);
    
    private void GetUserSelectedEquipment(Action onFinished, LootPicker rewardPicker)
    {
        var selectedRarity = rewardPicker.RandomRarity();
        var rewardEquips = new List<Equipment>();
        
        var possibleEquips = new Queue<Equipment>(rewardPicker.PickEquipments(equipmentPrizePool, 20, selectedRarity));
        while (rewardEquips.Count < 3)
        {
            var nextEquipment = possibleEquips.Dequeue();
            if (!rewardEquips.Any(x => x.Description.Equals(nextEquipment.Description)))
                rewardEquips.Add(nextEquipment);
        }

        Message.Publish(new GetUserSelectedEquipment(rewardEquips.ToArray().Shuffled(), equipment =>
        {
            equipment.IfPresent(e =>
            {
                AllMetrics.PublishGearRewardSelection(e.GetMetricNameOrDescription(), rewardEquips.Select(r => r.GetMetricNameOrDescription()).ToArray());
                state.SetRewardEquipment(e);
            });
            onFinished();
        }));
    }
}
