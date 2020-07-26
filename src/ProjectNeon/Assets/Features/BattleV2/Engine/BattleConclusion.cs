
using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;

    private void Advance()
    {
        if (adventure.IsFinalStageSegment)
        {
            Debug.Log("Navigating to victory screen");
            navigator.NavigateToVictoryScene();
        }
        else
        {
            Debug.Log("Advancing to next Stage Segment.");
            adventure.Advance();
            navigator.NavigateToGameScene();
        }
    }
    
    protected override void Execute(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Party)
            Advance();
        else
            navigator.NavigateToDefeatScene();
    }
}
