using System;
using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private AdventureProgress2 adventure2;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;
    [SerializeField] private float secondsBeforeGameOverScreen = 3f;
    [SerializeField] private BattleState state;
    [SerializeField] private BattleRewards eliteReward;
    [SerializeField] private BattleRewards normalReward;
    [SerializeField] private CurrentGameMap3 gameMap;
    
    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        if (state.IsEliteBattle)
            eliteReward.GrantVictoryRewardsAndThen(onFinished);
        else
            normalReward.GrantVictoryRewardsAndThen(onFinished);
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
            conclusion.Set(true, adventure2.CurrentAdventure.VictoryConclusion);
            this.ExecuteAfterDelay(() => navigator.NavigateToConclusionScene(), secondsBeforeReturnToAdventure);
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
            conclusion.Set(false, adventure2.CurrentAdventure.DefeatConclusion);
            this.ExecuteAfterDelay(() => navigator.NavigateToConclusionScene(), secondsBeforeGameOverScreen);
        }
    }
}
