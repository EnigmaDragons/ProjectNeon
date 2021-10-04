using System;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/CardMoneyValue")]
public class CardMoneyValueReward : BattleRewards
{
    [SerializeField] private AdventureProgress2 adventure2;
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delayBeforeProceed = new FloatReference(3f);
    
    public override void GrantVictoryRewardsAndThen(Action onFinished)
    {
        var rewardPicker = adventure2.CreateLootPicker(state.Party);
        var rarity = rewardPicker.RandomRarity();
        var credits = rarity.CardShopPrice();
        Log.Info($"Reward: Card Money Value - Rarity {rarity} - Value {credits}");
        state.AddRewardCredits(credits);
        Message.Publish(new ShowCreditsGain(rarity, state.RewardCredits));
        Message.Publish(new ExecuteAfterDelayRequested(delayBeforeProceed, onFinished));
    }
}