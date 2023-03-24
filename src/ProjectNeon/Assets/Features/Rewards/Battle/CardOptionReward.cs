using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRewards/CardOption")]
public class CardOptionReward : BattleRewards
{
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    [SerializeField] private DeterminedNodeInfo nodeInfo;
    
    public override void GrantVictoryRewardsAndThen(Action onFinished, LootPicker lootPicker) 
        => GetUserSelectedRewardCard(onFinished, lootPicker);

    private void GetUserSelectedRewardCard(Action onFinished, LootPicker rewardPicker)
    {
        if (state.Party.BaseHeroes.All(x => !x.Archetypes.Any()))
        {
            Message.Publish(new ExecuteAfterDelayRequested(0.5f, onFinished));
            return;
        }

        if (nodeInfo.CardRewardOptions.IsMissing)
        {
            var selectedRarity = rewardPicker.RandomRarity();
            nodeInfo.CardRewardOptions = rewardPicker.PickCards(cardPrizePool, 3, selectedRarity).Cast<CardType>().ToArray().Shuffled();
            Message.Publish(new SaveDeterminationsRequested());
        }
        
        var rewardCards = nodeInfo.CardRewardOptions.Value.Select(c => c.ToNonBattleCard(state.Party)).ToArray();
        Message.Publish(new GetUserSelectedCard(rewardCards, card =>
        {
            card.IfPresent(c =>
            {
                AllMetrics.PublishCardRewardSelection(c.Name, rewardCards.Select(r => r.Name).ToArray());
                state.SetRewardCards(c.BaseType);
            });
            nodeInfo.CardRewardOptions = Maybe<CardType[]>.Missing();
            onFinished();
        }));
    }
}
