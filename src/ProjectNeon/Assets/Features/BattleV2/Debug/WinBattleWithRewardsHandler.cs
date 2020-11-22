using System.Linq;
using UnityEngine;

public class WinBattleWithRewardsHandler : OnMessage<WinBattleWithRewards>
{
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    
    protected override void Execute(WinBattleWithRewards msg)
    {            
        var rewardPicker = new ShopSelectionPicker();
        state.SetRewardCards(rewardPicker.PickCards(state.Party, cardPrizePool, 2));
        state.AddRewardCredits(state.EnemyArea.Enemies.Sum(e => e.GetRewardCredits(state.PowerLevelRewardFactor)));
        state.Wrapup();
        Message.Publish(new BattleFinished(TeamType.Party));
    }
}
