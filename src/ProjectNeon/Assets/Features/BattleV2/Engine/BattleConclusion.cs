using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private IntReference levelUpPoints = new IntReference(8);
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;

    private void Advance()
    {
        if (adventure.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory screen");
            this.ExecuteAfterDelay(() => navigator.NavigateToVictoryScene(), secondsBeforeReturnToAdventure);
        }
        else
        {
            if (adventure.IsLastSegmentOfStage)
            {
                Log.Info("Party is levelling up");
                party.AwardLevelUpPoints(levelUpPoints);
            }
            Log.Info("Advancing to next Stage Segment.");
            adventure.Advance();
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
