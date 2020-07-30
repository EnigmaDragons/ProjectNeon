using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;

    private void Advance()
    {
        if (adventure.IsFinalStageSegment)
        {
            Debug.Log("Navigating to victory screen");
            this.ExecuteAfterDelay(() => navigator.NavigateToVictoryScene(), secondsBeforeReturnToAdventure);
        }
        else
        {
            Debug.Log("Advancing to next Stage Segment.");
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
