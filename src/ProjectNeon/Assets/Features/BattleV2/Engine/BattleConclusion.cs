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
    [SerializeField] private CurrentGameMap2 gameMap;
    
    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        var rewardFactors = adventure2.CurrentChapterNumber > 0
            ? adventure2.CurrentChapter.RewardRarityFactors
            : new DefaultRarityFactors();
        
        var rewardPicker = new ShopSelectionPicker(adventure2.CurrentChapterNumber, rewardFactors, state.Party);
        if (state.IsEliteBattle)
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
                equipment.IfPresent(e => state.SetRewardEquipment(e));
                onFinished();
            }));
        }
        else
        {
            var rewardCards = rewardPicker.PickCards(cardPrizePool, 3, RarityExtensions.AllExceptStarters);
            Message.Publish(new GetUserSelectedCard(rewardCards, card =>
            {
                card.IfPresent(c => state.SetRewardCards(c));
                onFinished();
            }));
        }
    }
    
    private void Advance()
    {
        if (adventure2.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory screen");
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToVictoryScene(), secondsBeforeReturnToAdventure);
        }
        else
        {
            Log.Info("Advancing to next Stage Segment.");
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
            CurrentGameData.Clear();
            this.ExecuteAfterDelay(() => navigator.NavigateToDefeatScene(), secondsBeforeReturnToAdventure);
        }
    }
}
