using System;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/PrefactoredReward")]
public class PrefactoredReward : BattleRewards
{
    [SerializeField] private BattleState s;
    [SerializeField] private FloatReference delayBeforeProceed;
    
    public override void GrantVictoryRewardsAndThen(Action onFinished, LootPicker lootPicker)
    {
        var credits = s.RewardCredits;
        var clinicVouchers = s.RewardClinicVouchers;
        Log.Info($"Reward: Prefactored Credits {credits} Clinic Vouchers {clinicVouchers}");
        Message.Publish(new ShowPrefactoredReward(credits, clinicVouchers));
        Message.Publish(new ExecuteAfterDelayRequested(delayBeforeProceed, onFinished));
    }
}
