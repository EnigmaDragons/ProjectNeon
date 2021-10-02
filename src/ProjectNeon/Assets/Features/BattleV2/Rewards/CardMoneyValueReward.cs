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
        state.AddRewardCredits(rarity.CardShopPrice());
        Message.Publish(new ExecuteAfterDelayRequested(delayBeforeProceed, onFinished));
    }
}