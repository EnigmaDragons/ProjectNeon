
public static class GameWrapup
{
    public static void NavigateToVictoryScreen(CurrentAdventureProgress p, CurrentAdventure a, Navigator n, AdventureConclusionState c)
    {
        Log.Info("Navigating to victory screen");
        p.AdventureProgress.Advance();
        AllMetrics.PublishGameWon();
        Message.Publish(new AutoSaveRequested());
        c.Set(true, a.Adventure.VictoryConclusion, CurrentGameData.Data.Stats);
        CurrentGameData.Clear();
        n.NavigateToConclusionScene();
    }
}