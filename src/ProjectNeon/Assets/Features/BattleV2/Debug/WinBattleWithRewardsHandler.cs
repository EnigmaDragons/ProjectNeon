using System.Linq;
using UnityEngine;

public class WinBattleWithRewardsHandler : OnMessage<WinBattleWithRewards>
{
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    
    protected override void Execute(WinBattleWithRewards msg)
    {            
        var rewardPicker = new ShopSelectionPicker();
        state.AddRewardCredits(state.EnemyArea.Enemies.Sum(e => e.GetRewardCredits(state.PowerLevelRewardFactor)));
        Message.Publish(new GetUserSelectedCard(rewardPicker.PickCards(state.Party, cardPrizePool, 3), card =>
        {
            card.IfPresent(c => state.SetRewardCards(c));
            Message.Publish(new BattleFinished(TeamType.Party));
            state.Wrapup();
        }));
    }
}
