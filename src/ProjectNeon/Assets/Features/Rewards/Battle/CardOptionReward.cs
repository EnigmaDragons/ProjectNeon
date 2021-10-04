using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/CardOption")]
public class CardOptionReward : BattleRewards
{
    [SerializeField] private AdventureProgress2 adventure2;
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    
    public override void GrantVictoryRewardsAndThen(Action onFinished) 
        => GetUserSelectedRewardCard(onFinished, adventure2.CreateLootPicker(state.Party));

    private void GetUserSelectedRewardCard(Action onFinished, LootPicker rewardPicker)
    {
        var selectedRarity = rewardPicker.RandomRarity();
        var rewardCardTypes = rewardPicker.PickCards(cardPrizePool, 3, selectedRarity);
        var rewardCards = rewardCardTypes.Select(c => c.ToNonBattleCard(state.Party)).ToArray().Shuffled();
        Message.Publish(new GetUserSelectedCard(rewardCards, card =>
        {
            card.IfPresent(c =>
            {
                AllMetrics.PublishCardRewardSelection(c.Name, rewardCards.Select(r => r.Name).ToArray());
                state.SetRewardCards(c.BaseType);
            });
            onFinished();
        }));
    }
}
