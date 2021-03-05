using System.Linq;
using UnityEngine;

public class WinBattleWithRewardsHandler : OnMessage<WinBattleWithRewards>
{
    [SerializeField] private BattleConclusion conclusion;
    [SerializeField] private BattleState state;
    
    protected override void Execute(WinBattleWithRewards msg)
    {            
        state.AddRewardCredits(state.EnemyArea.Enemies.Sum(e => e.GetRewardCredits(state.CreditPerPowerLevelRewardFactor)));
        state.AddRewardXp(state.EnemyArea.Enemies.Sum(e => e.GetRewardXp(state.XpPerPowerLevelRewardFactor)));
        conclusion.GrantVictoryRewardsAndThen(() =>
        {
            Message.Publish(new BattleFinished(TeamType.Party));
            state.Wrapup();
        });
    }
}
