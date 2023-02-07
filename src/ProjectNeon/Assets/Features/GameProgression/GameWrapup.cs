
using System.Linq;

public static class GameWrapup
{
    public static void NavigateToVictoryScreen(CurrentAdventureProgress p, CurrentAdventure a, Navigator n, AdventureConclusionState c, Hero[] heroes)
    {
        Log.Info("Navigating to victory screen");
        p.AdventureProgress.Advance();
        AllMetrics.PublishGameWon(p.AdventureProgress.AdventureId);
        CurrentProgressionData.RecordCompletedAdventure(p.AdventureProgress.AdventureId, 0, heroes.Select(h => h.Character.Id).ToArray());
        Achievements.RecordAdventureCompleted(p.AdventureProgress.AdventureId, true, p.AdventureProgress.Difficulty, heroes.Select(h => h.Character.NameTerm().ToEnglish()).ToArray());
        Message.Publish(new AutoSaveRequested());
        c.RecordFinishedGameAndCleanUp(true, a.Adventure.VictoryConclusionTerm, CurrentGameData.Data.Stats, heroes);
        if (a.Adventure.ShouldRollCreditsBeforeConclusionScene)
            n.NavigateToFinalCreditsScene();
        else
            n.NavigateToConclusionScene();
    }
}
