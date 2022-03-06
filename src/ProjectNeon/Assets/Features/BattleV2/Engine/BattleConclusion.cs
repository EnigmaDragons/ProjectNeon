using System;
using Features.GameProgression;
using UnityEngine;

public class BattleConclusion : OnMessage<BattleFinished>
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private Navigator navigator;
    [SerializeField] private float secondsBeforeReturnToAdventure = 2f;
    [SerializeField] private float secondsBeforeGameOverScreen = 3f;
    [SerializeField] private BattleState state;

    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        Message.Publish(new BattleRewardsStarted());
        if (adventureProgress.AdventureProgress.IsFinalStageSegment)
            Advance();
        else if (state.IsEliteBattle)
            adventure.Adventure.EliteBattleRewards.GrantVictoryRewardsAndThen(onFinished, adventureProgress.AdventureProgress.CreateLootPicker(state.Party));
        else
            adventure.Adventure.NormalBattleRewards.GrantVictoryRewardsAndThen(onFinished, adventureProgress.AdventureProgress.CreateLootPicker(state.Party));
    }
    
    private void Advance()
    {
        if (state.IsTutorialCombat)
        {
            Log.Info("Returning to academy from tutorial combat");
            this.ExecuteAfterDelay(() => navigator.NavigateToAcademyScene(), secondsBeforeReturnToAdventure);
        }
        else if (state.IsStoryEventCombat)
        {
            Log.Info("Returning to map from event combat");
            Message.Publish(new AutoSaveRequested());
            var adventureType = CurrentGameData.Data.AdventureProgress.Type;
            if (adventureType == GameAdventureProgressType.V2)
                this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
            if (adventureType == GameAdventureProgressType.V4)
                this.ExecuteAfterDelay(() => navigator.NavigateToGameSceneV4(), secondsBeforeReturnToAdventure);
        }
        else if (adventureProgress.AdventureProgress.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory screen");
            adventureProgress.AdventureProgress.Advance();
            AllMetrics.PublishGameWon();
            Message.Publish(new AutoSaveRequested());
            state.AccumulateRunStats();
            conclusion.Set(true, adventure.Adventure.VictoryConclusion, CurrentGameData.Data.Stats);
            CurrentGameData.Clear();
            this.ExecuteAfterDelay(() => navigator.NavigateToConclusionScene(), secondsBeforeGameOverScreen);
        }
        else
        {
            Log.Info("Advancing to next Stage Segment.");
            adventureProgress.AdventureProgress.Advance();
            Message.Publish(new AutoSaveRequested());
            var adventureType = CurrentGameData.Data.AdventureProgress.Type;
            if (adventureType == GameAdventureProgressType.V2)
                this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
            if (adventureType == GameAdventureProgressType.V4 || adventureType == GameAdventureProgressType.Unknown)
                this.ExecuteAfterDelay(() => navigator.NavigateToGameSceneV4(), secondsBeforeReturnToAdventure);
        }
    }

    protected override void Execute(BattleFinished msg)
    {
        Time.timeScale = 1;
        if (msg.Winner == TeamType.Party)
            Advance();
        else
        {
            Log.Info("Navigating to defeat screen");
            AllMetrics.PublishGameLost();
            state.AccumulateRunStats();
            conclusion.Set(false, adventure.Adventure.DefeatConclusion, CurrentGameData.Data.Stats);
            CurrentGameData.Clear();
            this.ExecuteAfterDelay(() => navigator.NavigateToConclusionScene(), secondsBeforeGameOverScreen);
        }
    }
}
