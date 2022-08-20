using System.Linq;
using UnityEngine;

public class WinBattleWithRewardsHandler : OnMessage<WinBattleWithRewards>
{
    [SerializeField] private BattleConclusion conclusion;
    [SerializeField] private BattleState state;

    private bool _triggered = false;
    
    protected override void Execute(WinBattleWithRewards msg)
    {
        if (_triggered)
            return;
        
        _triggered = true;
        var rewardCredits = state.EnemyArea.Enemies.Sum(e => e.GetRewardCredits(state.CreditPerPowerLevelRewardFactor));
        var rewardXp = state.EnemyArea.Enemies.Sum(e => e.GetRewardXp(state.XpPerPowerLevelRewardFactor));
        state.AddRewardCredits(rewardCredits);
        state.AddRewardXp(rewardXp);
        var startedOnFinish = false;
        conclusion.GrantVictoryRewardsAndThen(() =>
        {
            if (startedOnFinish)
                return;

            startedOnFinish = true;
            Message.Publish(new BattleFinished(TeamType.Party));
            state.Wrapup();
        });
    }
}
