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
