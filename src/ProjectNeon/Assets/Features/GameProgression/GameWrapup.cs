
using System.Linq;

public static class GameWrapup
{
    public static void NavigateToVictoryScreen(CurrentAdventureProgress p, CurrentAdventure a, Navigator n, AdventureConclusionState c, HeroCharacter[] heroes)
    {
        Log.Info("Navigating to victory screen");
        p.AdventureProgress.Advance();
        AllMetrics.PublishGameWon(p.AdventureProgress.AdventureId);
        CurrentProgressionData.RecordCompletedAdventure(p.AdventureProgress.AdventureId, 0, heroes.Select(h => h.Id).ToArray());
        Message.Publish(new AutoSaveRequested());
        c.Set(true, a.Adventure.VictoryConclusion, CurrentGameData.Data.Stats, heroes);
        CurrentGameData.Clear();
        n.NavigateToConclusionScene();
    }
}
