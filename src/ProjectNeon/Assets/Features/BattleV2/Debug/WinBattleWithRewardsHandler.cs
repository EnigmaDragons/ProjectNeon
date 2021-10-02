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
        state.AddRewardCredits(state.EnemyArea.Enemies.Sum(e => e.GetRewardCredits(state.CreditPerPowerLevelRewardFactor)));
        state.AddRewardXp(state.EnemyArea.Enemies.Sum(e => e.GetRewardXp(state.XpPerPowerLevelRewardFactor)));
        conclusion.GrantVictoryRewardsAndThen(() =>
        {
            Message.Publish(new BattleFinished(TeamType.Party));
            state.Wrapup();
        });
    }
}
