using System;
using Features.GameProgression.Messages;
using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private IntReference levelUpPoints = new IntReference(8);
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private AdventureProgress2 adventure2;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private BattleState state;
    [SerializeField] private ShopCardPool cardPrizePool;
    [SerializeField] private EquipmentPool equipmentPrizePool;

    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        var rewardPicker = new ShopSelectionPicker();
        if (state.IsEliteBattle)
        {
            var rewardEquipments = rewardPicker.PickEquipments(state.Party, equipmentPrizePool, 3);
            Message.Publish(new GetUserSelectedEquipment(rewardEquipments, equipment =>
            {
                equipment.IfPresent(e => state.SetRewardEquipment(e));
                onFinished();
            }));
        }
        else
        {
            var rewardCards = rewardPicker.PickCards(state.Party, cardPrizePool, 3);
            Message.Publish(new GetUserSelectedCard(rewardCards, card =>
            {
                card.IfPresent(c => state.SetRewardCards(c));
                onFinished();
            }));
        }
    }
    
    private void Advance()
    {
        if (currentAdventure.Adventure.IsV2 ? adventure2.IsFinalStageSegment : adventure.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory screen");
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToVictoryScene(), secondsBeforeReturnToAdventure);
        }
        else
        {
            if (currentAdventure.Adventure.IsV2 ? adventure2.IsLastSegmentOfStage : adventure.IsLastSegmentOfStage)
            {
                Log.Info("Party is levelling up");
                party.AwardLevelUpPoints(levelUpPoints);
            }
            Log.Info("Advancing to next Stage Segment.");
            if (currentAdventure.Adventure.IsV2)
                adventure2.Advance();
            else
                adventure.Advance();
            Message.Publish(new AutoSaveRequested());
            this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
        }
    }
    
    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Party)
            Advance();
        else
            this.ExecuteAfterDelay(() => navigator.NavigateToDefeatScene(), secondsBeforeReturnToAdventure);
    }
}
