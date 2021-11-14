using System;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/CardMoneyValue")]
public class CardMoneyValueReward : BattleRewards
{
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delayBeforeProceed = new FloatReference(3f);
    
    public override void GrantVictoryRewardsAndThen(Action onFinished, LootPicker lootPicker)
    {
        var rarity = lootPicker.RandomRarity();
        var credits = rarity.CardShopPrice();
        Log.Info($"Reward: Card Money Value - Rarity {rarity} - Value {credits}");
        state.AddRewardCredits(credits);
        Message.Publish(new ShowCreditsGain(rarity, state.RewardCredits));
        Message.Publish(new ExecuteAfterDelayRequested(delayBeforeProceed, onFinished));
    }
}