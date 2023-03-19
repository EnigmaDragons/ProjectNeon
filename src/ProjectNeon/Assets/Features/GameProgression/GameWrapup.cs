using System.Linq;

public static class GameWrapup
{
    public static void NavigateToVictoryScreen(CurrentAdventureProgress p, CurrentAdventure a, CurrentBoss b, Navigator n, AdventureConclusionState c, Hero[] heroes)
    {
        Log.Info("Navigating to victory screen");
        p.AdventureProgress.Advance();
        AllMetrics.PublishGameWon(p.AdventureProgress.AdventureId);
        
        var bossId = -1;
        if (b == null)
            Log.Error("Current Boss is null");
        else
            bossId = p.AdventureProgress.FinalBoss != null ? p.AdventureProgress.FinalBoss.id : a.Adventure.BossSelection ? b.Boss.id : -1;
        
        CurrentProgressionData.RecordCompletedAdventure(p.AdventureProgress.AdventureId, p.AdventureProgress.Difficulty.id, bossId, heroes.Select(h => h.Character.Id).ToArray());
        Achievements.RecordAdventureCompleted(p.AdventureProgress.AdventureId, true, p.AdventureProgress.Difficulty, heroes.Select(h => h.Character.NameTerm().ToEnglish()).ToArray(), p.AdventureProgress.StoryStates);
        Message.Publish(new AutoSaveRequested());
        c.RecordFinishedGameAndCleanUp(true, a.Adventure.VictoryConclusionTerm, CurrentGameData.Data.Stats, heroes);
        if (a.Adventure.ShouldRollCreditsBeforeConclusionScene)
            n.NavigateToFinalCreditsScene();
        else
            n.NavigateToConclusionScene();
    }
}
