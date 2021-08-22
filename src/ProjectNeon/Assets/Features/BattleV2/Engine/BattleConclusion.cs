using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private AdventureProgress2 adventure2;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    [SerializeField] private EquipmentPool equipmentPrizePool;
    [SerializeField] private CurrentGameMap3 gameMap;
    
    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        var rewardPicker = adventure2.CreateLootPicker(state.Party);
        if (state.IsEliteBattle)
            GetUserSelectedEquipment(onFinished, rewardPicker);
        else
            GetUserSelectedRewardCard(onFinished, rewardPicker);
    }

    private void GetUserSelectedEquipment(Action onFinished, LootPicker rewardPicker)
    {
        // Tuned Reward Set
        var rewardEquips = rewardPicker
            .PickEquipments(equipmentPrizePool, 1, Rarity.Uncommon, Rarity.Rare, Rarity.Epic)
            .ToList();

        var possibleEquips = new Queue<Equipment>(rewardPicker.PickEquipments(equipmentPrizePool, 20));
        while (rewardEquips.Count < 3)
        {
            var nextEquipment = possibleEquips.Dequeue();
            if (!rewardEquips.Any(x => x.Description.Equals(nextEquipment.Description)))
                rewardEquips.Add(nextEquipment);
        }

        Message.Publish(new GetUserSelectedEquipment(rewardEquips.ToArray().Shuffled(), equipment =>
        {
            equipment.IfPresent(e =>
            {
                AllMetrics.PublishGearRewardSelection(e.GetMetricNameOrDescription(), rewardEquips.Select(r => r.GetMetricNameOrDescription()).ToArray());
                state.SetRewardEquipment(e);
            });
            onFinished();
        }));
    }

    private void GetUserSelectedRewardCard(Action onFinished, LootPicker rewardPicker)
    {
        var rewardCardTypes = rewardPicker.PickCards(cardPrizePool, 3, RarityExtensions.AllExceptStarters);
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

    private void Advance()
    {
        if (state.IsStoryEventCombat)
        {
            Log.Info("Returning to map from event combat");
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
        }
        else if (adventure2.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory screen");
            gameMap.CompleteCurrentNode();
            AllMetrics.PublishGameWon();
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToVictoryScene(), secondsBeforeReturnToAdventure);
        }
        else
        {
            Log.Info("Advancing to next Stage Segment.");
            gameMap.CompleteCurrentNode();
            adventure2.AdvanceStageIfNeeded();
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
        }
    }

    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Party)
            Advance();
        else
        {
            Log.Info("Navigating to defeat screen");
            AllMetrics.PublishGameLost();
            CurrentGameData.Clear();
            this.ExecuteAfterDelay(() => navigator.NavigateToDefeatScene(), secondsBeforeReturnToAdventure);
        }
    }
}
