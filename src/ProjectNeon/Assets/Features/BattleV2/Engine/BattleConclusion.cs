using System;
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
    [SerializeField] private BoolReference useNewTutorialFlow;
    [SerializeField] private SaveLoadSystem saveLoadSystem;
    [SerializeField] private PartyAdventureState partyState;
    [SerializeField] private CurrentBoss boss;

    public void GrantVictoryRewardsAndThen(Action onFinished)
    {
        Message.Publish(new BattleRewardsStarted());
        if (adventureProgress.HasActiveAdventure && (!state.IsTutorialCombat || useNewTutorialFlow.Value))
            if (adventureProgress.AdventureProgress.IsFinalStageSegment || adventureProgress.AdventureProgress.IsFinalBoss)
                onFinished();
            else if (state.IsEliteBattle)
                adventure.Adventure.EliteBattleRewards.GrantVictoryRewardsAndThen(onFinished, adventureProgress.AdventureProgress.CreateLootPicker(state.Party));
            else
                adventure.Adventure.NormalBattleRewards.GrantVictoryRewardsAndThen(onFinished, adventureProgress.AdventureProgress.CreateLootPicker(state.Party));
        else
            onFinished();
    }
    
    private void Advance()
    {
        if (state.IsTutorialCombat && !useNewTutorialFlow.Value)
        {
            Log.Info("Returning to academy from tutorial combat");
            this.ExecuteAfterDelay(() => Message.Publish(new NavigateToNextTutorialFlow()), secondsBeforeReturnToAdventure);
        }
        else if (state.IsStoryEventCombat)
        {
            Log.Info("Returning to map from event combat");
            Message.Publish(new AutoSaveRequested());
            var adventureType = CurrentGameData.Data.AdventureProgress.Type;
            ReturnToGameScene(adventureType);
        }
        else if (adventureProgress.AdventureProgress.IsFinalStageSegment)
        {
            if (state.IsTutorialCombat || !CurrentAcademyData.Data.IsLicensedBenefactor)
            {
                TutorialWonHandler.Execute(secondsBeforeGameOverScreen);
            }
            else
            {
                state.AccumulateRunStats();
                this.ExecuteAfterDelay(() => GameWrapup.NavigateToVictoryScreen(adventureProgress, adventure, boss, navigator, conclusion, partyState.Heroes), secondsBeforeGameOverScreen);   
            }
        }
        else
        {
            Log.Info("Advancing to next Stage Segment.");
            adventureProgress.AdventureProgress.Advance();
            Message.Publish(new AutoSaveRequested());
            var adventureType = CurrentGameData.Data.AdventureProgress.Type;
            ReturnToGameScene(adventureType);
        }
    }

    private void ReturnToGameScene(GameAdventureProgressType adventureType)
    {
        if (adventureType == GameAdventureProgressType.V2)
            this.ExecuteAfterDelay(() => navigator.NavigateToGameScene(), secondsBeforeReturnToAdventure);
        if (adventureType == GameAdventureProgressType.V4)
            this.ExecuteAfterDelay(() => navigator.NavigateToGameSceneV4(), secondsBeforeReturnToAdventure);
        if (adventureType == GameAdventureProgressType.V5)
            this.ExecuteAfterDelay(() => navigator.NavigateToGameSceneV5(), secondsBeforeReturnToAdventure);
    }

    protected override void Execute(BattleFinished msg)
    {
        Time.timeScale = 1;
        if (msg.Winner == TeamType.Party)
            Advance();
        else if ((state.IsTutorialCombat && useNewTutorialFlow.Value) || adventureProgress.AdventureProgress.Difficulty.ResetAfterDeath)
        {
            Log.Info("Restarting Battle");
            Achievements.RecordAdventureCompleted(adventure.Adventure.Id, false, adventureProgress.AdventureProgress.Difficulty, Array.Empty<string>(), adventureProgress.AdventureProgress.StoryStates);
            saveLoadSystem.LoadSavedGame();
            partyState.Heroes.ForEach(x => x.SetHp(x.Stats.MaxHp()));
            this.ExecuteAfterDelay(() => navigator.NavigateToGameSceneV5(), secondsBeforeGameOverScreen);
        }
        else
        {
            Log.Info("Navigating to defeat screen");
            Achievements.RecordAdventureCompleted(adventure.Adventure.Id, false, adventureProgress.AdventureProgress.Difficulty, Array.Empty<string>(), adventureProgress.AdventureProgress.StoryStates);
            AllMetrics.PublishGameLost(adventure.Adventure.Id);
            state.AccumulateRunStats();
            conclusion.RecordFinishedGameAndCleanUp(false, adventure.Adventure.DefeatConclusionTerm, CurrentGameData.Data.Stats, partyState.Heroes);
            this.ExecuteAfterDelay(() => navigator.NavigateToConclusionScene(), secondsBeforeGameOverScreen);
        }
    }
}
